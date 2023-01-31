namespace MyBudget.UI.Infrastructure.Routes
{
    public static class OrderEndpoints
    {
        public static string GetAllPaged(int pageNumber, int pageSize, string searchString, string[] orderBy)
        {
            string url = $"api/v1/Order?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
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
        public static string GelAllAutocompleteData = "api/v1/Order/GetAllSelectView";
        public static string GetCount = "api/v1/Order/count";
        public static string GetOrderById(int patientId)
        {
            return $"api/v1/Order/{patientId}";
        }


        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string Save = "api/v1/Order";
        public static string Delete = "api/v1/Order";
        public static string Export = "api/v1/Order/export";
        public static string ChangePassword = "api/identity/account/changepassword";
        public static string UpdateProfile = "api/identity/account/updateprofile";
    }
}
