using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Libraries
    {
        public const string Vconf = "libvconf.so.0";
    }

    internal static partial class Vconf
    {
        #region Set, Get for keyname
        // API int vconf_set_int(const char* in_key, const int intval)
        [DllImport(Interop.Libraries.Vconf, EntryPoint = "vconf_set_int", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int VconfSetInt(string in_key, int intval);

        // API int vconf_get_int(const char *in_key, int *intval)
        [DllImport(Interop.Libraries.Vconf, EntryPoint = "vconf_get_int", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int VconfGetInt(string in_key, out int intval);

        // API int vconf_set_bool(const char* in_key, const int boolval)
        [DllImport(Interop.Libraries.Vconf, EntryPoint = "vconf_set_bool", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int VconfSetBool(string in_key, int boolval);

        // API int vconf_get_bool(const char *in_key, int *boolval)
        [DllImport(Interop.Libraries.Vconf, EntryPoint = "vconf_get_bool", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int VconfGetBool(string in_key, out int boolval);

        // API int vconf_set_dbl(const char* in_key, const double dblval)
        [DllImport(Interop.Libraries.Vconf, EntryPoint = "vconf_set_dbl", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int VconfSetDbl(string in_key, double dblval);

        // API int vconf_get_dbl(const char *in_key, double *dblval)
        [DllImport(Interop.Libraries.Vconf, EntryPoint = "vconf_get_dbl", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int VconfGetDbl(string in_key, out double dblval);

        // API int vconf_set_str(const char* in_key, const char* strval)
        [DllImport(Interop.Libraries.Vconf, EntryPoint = "vconf_set_str", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int VconfSetString(string in_key, string strval);

        // API char *vconf_get_str(const char *in_key)
        [DllImport(Interop.Libraries.Vconf, EntryPoint = "vconf_get_str", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr VconfGetString(string in_key);
        #endregion

        #region Nofify/Ignore
        // API int vconf_notify_key_changed(const char *in_key, vconf_callback_fn cb, void *user_data)
        [DllImport(Interop.Libraries.Vconf, EntryPoint = "vconf_notify_key_changed", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int VconfNotifyKeyChanged(string inKey, VconfCallback callback, IntPtr userData);

        // API int vconf_ignore_key_changed(const char *in_key, vconf_callback_fn cb)
        [DllImport(Interop.Libraries.Vconf, EntryPoint = "vconf_ignore_key_changed", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int VconfIgnoreKeyChanged(string inKey, VconfCallback callback);

        // typedef void (*vconf_callback_fn) (keynode_t *node, void *user_data);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void VconfCallback(IntPtr node, IntPtr userData);
        #endregion

        #region Structs
        [StructLayout(LayoutKind.Explicit)]
        internal struct KeyNode
        {
            [FieldOffset(0)]
            public string KeyName;

            [FieldOffset(4)]
            public Type Type;

            [FieldOffset(8)]
            public int ValueInt;
            [FieldOffset(8)]
            public int ValueBool;
            [FieldOffset(8)]
            public double ValueDouble;
            [FieldOffset(8)]
            public IntPtr ValueString;

            [FieldOffset(12)]
            public IntPtr NextNode;
        };
        #endregion

        #region Enums
        internal enum Type : int
        {
            VCONF_TYPE_NONE = 0,    /**< Vconf none type for Error detection */
            VCONF_TYPE_STRING = 40, /**< Vconf string type */
            VCONF_TYPE_INT = 41,    /**< Vconf integer type */
            VCONF_TYPE_DOUBLE = 42, /**< Vconf double type */
            VCONF_TYPE_BOOL = 43,   /**< Vconf boolean type */
            VCONF_TYPE_DIR          /**< Vconf directory type */
        };
        #endregion
    }
}