using Microsoft.AspNetCore.Http;

namespace YasamPsikologProject.WebUi.Helpers
{
    public static class SessionHelper
    {
        // Session Keys
        private const string USER_ID_KEY = "UserId";
        private const string USER_ROLE_KEY = "UserRole";
        private const string PSYCHOLOGIST_ID_KEY = "PsychologistId";
        private const string USER_NAME_KEY = "UserName";
        private const string USER_EMAIL_KEY = "UserEmail";

        /// <summary>
        /// Session'dan kullanıcı ID'sini alır
        /// </summary>
        public static int? GetUserId(this ISession session)
        {
            return session.GetInt32(USER_ID_KEY);
        }

        /// <summary>
        /// Session'a kullanıcı ID'sini kaydeder
        /// </summary>
        public static void SetUserId(this ISession session, int userId)
        {
            session.SetInt32(USER_ID_KEY, userId);
        }

        /// <summary>
        /// Session'dan kullanıcı rolünü alır
        /// </summary>
        public static string? GetUserRole(this ISession session)
        {
            return session.GetString(USER_ROLE_KEY);
        }

        /// <summary>
        /// Session'a kullanıcı rolünü kaydeder
        /// </summary>
        public static void SetUserRole(this ISession session, string role)
        {
            session.SetString(USER_ROLE_KEY, role);
        }

        /// <summary>
        /// Session'dan psikolog ID'sini alır
        /// </summary>
        public static int? GetPsychologistId(this ISession session)
        {
            return session.GetInt32(PSYCHOLOGIST_ID_KEY);
        }

        /// <summary>
        /// Session'a psikolog ID'sini kaydeder
        /// </summary>
        public static void SetPsychologistId(this ISession session, int psychologistId)
        {
            session.SetInt32(PSYCHOLOGIST_ID_KEY, psychologistId);
        }

        /// <summary>
        /// Session'dan kullanıcı adını alır
        /// </summary>
        public static string? GetUserName(this ISession session)
        {
            return session.GetString(USER_NAME_KEY);
        }

        /// <summary>
        /// Session'a kullanıcı adını kaydeder
        /// </summary>
        public static void SetUserName(this ISession session, string userName)
        {
            session.SetString(USER_NAME_KEY, userName);
        }

        /// <summary>
        /// Session'dan kullanıcı email'ini alır
        /// </summary>
        public static string? GetUserEmail(this ISession session)
        {
            return session.GetString(USER_EMAIL_KEY);
        }

        /// <summary>
        /// Session'a kullanıcı email'ini kaydeder
        /// </summary>
        public static void SetUserEmail(this ISession session, string email)
        {
            session.SetString(USER_EMAIL_KEY, email);
        }

        /// <summary>
        /// Kullanıcının psikolog olup olmadığını kontrol eder
        /// </summary>
        public static bool IsPsychologist(this ISession session)
        {
            var role = session.GetUserRole();
            return role != null && (role == "Psychologist" || role == "2");
        }

        /// <summary>
        /// Kullanıcının süper admin olup olmadığını kontrol eder
        /// </summary>
        public static bool IsSuperAdmin(this ISession session)
        {
            var role = session.GetUserRole();
            return role != null && (role == "SuperAdmin" || role == "1");
        }

        /// <summary>
        /// Kullanıcının danışan olup olmadığını kontrol eder
        /// </summary>
        public static bool IsClient(this ISession session)
        {
            var role = session.GetUserRole();
            return role != null && (role == "Client" || role == "3");
        }

        /// <summary>
        /// Kullanıcının giriş yapıp yapmadığını kontrol eder
        /// </summary>
        public static bool IsAuthenticated(this ISession session)
        {
            return session.GetUserId().HasValue;
        }

        /// <summary>
        /// Session'ı temizler (Logout)
        /// </summary>
        public static void ClearSession(this ISession session)
        {
            session.Clear();
        }
    }
}
