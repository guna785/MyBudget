using Microsoft.Extensions.Localization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Shared.Wrapper;
using System.Data;
using System.Drawing;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace MyBudget.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        private readonly IStringLocalizer<ExcelService> _localizer;

        public ExcelService(IStringLocalizer<ExcelService> localizer)
        {
            _localizer = localizer;
        }

        public async Task<string> ExportAsync<TData>(IEnumerable<TData> data
            , Dictionary<string, Func<TData, object>> mappers
            , string sheetName = "Sheet1")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage p = new();
            p.Workbook.Properties.Author = "BlazorHero";
            _ = p.Workbook.Worksheets.Add(_localizer["Audit Trails"]);
            ExcelWorksheet ws = p.Workbook.Worksheets[0];
            ws.Name = sheetName;
            ws.Cells.Style.Font.Size = 11;
            ws.Cells.Style.Font.Name = "Calibri";

            int colIndex = 1;
            int rowIndex = 1;

            List<string> headers = mappers.Keys.Select(x => x).ToList();

            foreach (string? header in headers)
            {
                ExcelRange cell = ws.Cells[rowIndex, colIndex];

                ExcelFill fill = cell.Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(Color.LightBlue);

                Border border = cell.Style.Border;
                border.Bottom.Style =
                    border.Top.Style =
                        border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                cell.Value = header;

                colIndex++;
            }

            List<TData> dataList = data.ToList();
            foreach (TData? item in dataList)
            {
                colIndex = 1;
                rowIndex++;

                IEnumerable<object> result = headers.Select(header => mappers[header](item));

                foreach (object? value in result)
                {
                    ws.Cells[rowIndex, colIndex++].Value = value;
                }
            }

            using (ExcelRange autoFilterCells = ws.Cells[1, 1, dataList.Count + 1, headers.Count])
            {
                autoFilterCells.AutoFilter = true;
                autoFilterCells.AutoFitColumns();
            }

            byte[] byteArray = await p.GetAsByteArrayAsync();
            return Convert.ToBase64String(byteArray);
        }

        public async Task<IResult<IEnumerable<TEntity>>> ImportAsync<TEntity>(Stream stream, Dictionary<string, Func<DataRow, TEntity, object>> mappers, string sheetName = "Sheet1")
        {
            List<TEntity> result = new();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using ExcelPackage p = new();
            stream.Position = 0;
            await p.LoadAsync(stream);
            ExcelWorksheet ws = p.Workbook.Worksheets[sheetName];
            if (ws == null)
            {
                return await Result<IEnumerable<TEntity>>.FailAsync(string.Format(_localizer["Sheet with name {0} does not exist!"], sheetName));
            }

            DataTable dt = new();
            bool titlesInFirstRow = true;
            foreach (ExcelRangeBase? firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
            {
                _ = dt.Columns.Add(titlesInFirstRow ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");
            }
            int startRow = titlesInFirstRow ? 2 : 1;
            List<string> headers = mappers.Keys.Select(x => x).ToList();
            List<string> errors = new();
            foreach (string? header in headers)
            {
                if (!dt.Columns.Contains(header))
                {
                    errors.Add(string.Format(_localizer["Header '{0}' does not exist in table!"], header));
                }
            }

            if (errors.Any())
            {
                return await Result<IEnumerable<TEntity>>.FailAsync(errors);
            }

            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
            {
                try
                {
                    ExcelRange wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = dt.Rows.Add();
                    TEntity item = (TEntity)Activator.CreateInstance(typeof(TEntity))!;
                    foreach (ExcelRangeBase? cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                    headers.ForEach(x => mappers[x](row, item));
                    result.Add(item!);
                }
                catch (Exception e)
                {
                    return await Result<IEnumerable<TEntity>>.FailAsync(_localizer[e.Message]);
                }
            }

            return await Result<IEnumerable<TEntity>>.SuccessAsync(result, _localizer["Import Success"]);
        }
    }
}
