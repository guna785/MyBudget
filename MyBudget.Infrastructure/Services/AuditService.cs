using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MyBudget.Infrastructure.Contexts;
using MyBudget.Infrastructure.Models.Audit;
using MyBudget.Infrastructure.Specifications;
using MyBudget.Application.Extensions;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Application.Responses.Audit;
using MyBudget.Shared.Wrapper;
using System.Globalization;

namespace MyBudget.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<AuditService> _localizer;

        public AuditService(
            IMapper mapper,
            ApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<AuditService> localizer)
        {
            _mapper = mapper;
            _context = context;
            _excelService = excelService;
            _localizer = localizer;
        }

        public async Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync(string userId)
        {
            List<Audit> trails = await _context.AuditTrails.Where(a => a.UserId == userId).OrderByDescending(a => a.Id).Take(250).ToListAsync();
            List<AuditResponse> mappedLogs = _mapper.Map<List<AuditResponse>>(trails);
            return await Result<IEnumerable<AuditResponse>>.SuccessAsync(mappedLogs);
        }

        public async Task<IResult<string>> ExportToExcelAsync(string userId, string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false)
        {
            AuditFilterSpecification auditSpec = new(userId, searchString, searchInOldValues, searchInNewValues);
            List<Audit> trails = await _context.AuditTrails
                .Specify(auditSpec)
                .OrderByDescending(a => a.DateTime)
                .ToListAsync();
            string data = await _excelService.ExportAsync(trails, sheetName: _localizer["Audit trails"],
                mappers: new Dictionary<string, Func<Audit, object>>
                {
                    { _localizer["Table Name"], item => item.TableName },
                    { _localizer["Type"], item => item.Type },
                    { _localizer["Date Time (Local)"], item => DateTime.SpecifyKind(item.DateTime, DateTimeKind.Utc).ToLocalTime().ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["Date Time (UTC)"], item => item.DateTime.ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["Primary Key"], item => item.PrimaryKey },
                    { _localizer["Old Values"], item => item.OldValues },
                    { _localizer["New Values"], item => item.NewValues },
                });

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}
