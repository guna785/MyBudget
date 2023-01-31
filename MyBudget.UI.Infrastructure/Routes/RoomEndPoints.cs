namespace MyBudget.UI.Infrastructure.Routes
{
    public class RoomEndPoints
    {
        public static string GetAllPaged(int pageNumber, int pageSize, string searchString, string[] orderBy)
        {
            string url = $"api/v1/Room?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
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
        public static string GelAllAutocompleteData = "api/v1/Room/GetAllSelectView";
        public static string GetCount = "api/v1/Room/count";
        public static string GetRoomById(int RoomId)
        {
            return $"api/v1/Room/{RoomId}";
        }
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string Save = "api/v1/Room";
        public static string Delete = "api/v1/Room";
        public static string Export = "api/v1/Room/export";
        public static string ChangePassword = "api/identity/account/changepassword";
        public static string UpdateProfile = "api/identity/account/updateprofile";
    }
}
