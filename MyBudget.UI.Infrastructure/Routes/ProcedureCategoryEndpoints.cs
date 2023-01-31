namespace MyBudget.UI.Infrastructure.Routes
{
    public static class ProcedureCategoryEndpoints
    {
        public static string GetAllPaged(int pageNumber, int pageSize, string searchString, string[] orderBy)
        {
            string url = $"api/v1/ProcedureCategory?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
            if (orderBy?.Any() == true)
            {
                foreach (string orderByPart in orderBy)
                {
                    url += $"{orderByPart},";
                }
                url = url[..^1]; // loose training ,
            }
            return url;
        }
        public static string GelAllAutocompleteData = "api/v1/ProcedureCategory/GetAllSelectView";
        public static string GetCount = "api/v1/ProcedureCategory/count";
        public static string GetProcedureCategoryById(int patientId)
        {
            return $"api/v1/ProcedureCategory/{patientId}";
        }


        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string Save = "api/v1/ProcedureCategory";
        public static string Delete = "api/v1/ProcedureCategory";
        public static string Export = "api/v1/ProcedureCategory/export";
        public static string ChangePassword = "api/identity/account/changepassword";
        public static string UpdateProfile = "api/identity/account/updateprofile";
    }
}
