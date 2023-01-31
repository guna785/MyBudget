namespace MyBudget.UI.Infrastructure.Routes
{
    public static class OrganisationEndpoints
    {
        public static string GetAllPaged(int pageNumber, int pageSize, string searchString, string[] orderBy)
        {
            string url = $"api/v1/Organisation?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
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
        public static string GelAllAutocompleteData = "api/v1/Organisation/GetAllSelectView";
        public static string GetCount = "api/v1/Organisation/count";

        public static string GetOrganisationImage(int productId)
        {
            return $"api/v1/Organisation/image/{productId}";
        }

        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string Save = "api/v1/Organisation";
        public static string Delete = "api/v1/Organisation";
        public static string Export = "api/v1/Organisation/export";
        public static string ChangePassword = "api/identity/account/changepassword";
        public static string UpdateProfile = "api/identity/account/updateprofile";
    }
}
