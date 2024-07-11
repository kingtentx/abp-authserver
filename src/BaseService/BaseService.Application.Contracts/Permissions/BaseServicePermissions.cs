namespace BaseService.Permissions
{
    public static class BaseServicePermissions
    {
        public const string BaseService = "BaseService";
        #region Base
        public static class AuditLogging
        {
            public const string Default = BaseService + ".AuditLogging";
        }

        public static class DataDictionary
        {
            public const string Default = BaseService + ".DataDictionary";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        public static class DataDictionaryDetail
        {
            public const string Default = BaseService + ".DataDictionaryDetail";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        public static class Organization
        {
            public const string Default = BaseService + ".Organization";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        public static class Position
        {
            public const string Default = BaseService + ".Position";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        #endregion

        #region System
        public static class Menu
        {
            public const string Default = BaseService + ".Menu";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        public static class RoleMenu
        {
            public const string Default = BaseService + ".RoleMenu";
            public const string Update = Default + ".Update";
        }

        public static class Authority
        {
            public const string Default = BaseService + ".Authority";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        public static class Edge
        {
            public const string Default = BaseService + ".Edge";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }


        #endregion


    }
}
