using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Tizen
{
    public partial class Vconf
    {
        private static void Log(string message, [CallerFilePath] string file = "", [CallerMemberName] string func = "", [CallerLineNumber] int line = 0)
        {
            global::Tizen.Log.Verbose("VconfCS", message, file, func, line);
        }

        #region Setters/Getters
        public static bool SetBool(string key, bool value)
        {
            int result = Interop.Vconf.VconfSetBool(key, value ? 1 : 0);
            Log($"key: {key}, value: {value}, result: {result}");
            return result == 0;
        }

        public static bool TryGetBool(string key, out bool value)
        {
            int result = Interop.Vconf.VconfGetBool(key, out int intvalue);
            value = (intvalue == 1);
            Log($"key: {key}, value: {value}, result: {result}");
            return result == 0;
        }

        public static bool SetInt(string key, int value)
        {
            int result = Interop.Vconf.VconfSetInt(key, value);
            Log($"key: {key}, value: {value}, result: {result}");
            return result == 0;
        }

        public static bool TryGetInt(string key, out int value)
        {
            int result = Interop.Vconf.VconfGetInt(key, out value);
            Log($"key: {key}, value: {value}, result: {result}");
            return result == 0;
        }

        public static bool SetDouble(string key, double value)
        {
            int result = Interop.Vconf.VconfSetDbl(key, value);
            Log($"key: {key}, value: {value}, result: {result}");
            return result == 0;
        }

        public static bool TryGetDouble(string key, out double value)
        {
            int result = Interop.Vconf.VconfGetDbl(key, out value);
            Log($"key: {key}, value: {value}, result: {result}");
            return result == 0;
        }

        // Underlying CAPI function always returns -1, thus CSAPI would always return false. Even though the value is set properly!
        public static bool SetString(string key, string value)
        {
            int result = Interop.Vconf.VconfSetString(key, value);
            Log($"key: {key}, value: {value}, result: {result}");
            return result == 0;
        }

        public static bool TryGetString(string key, out string value)
        {
            IntPtr ptr = Interop.Vconf.VconfGetString(key);
            value = Marshal.PtrToStringAnsi(ptr);
            Log($"key: {key}, value: {value}, intptr: {ptr}");
            return ptr == IntPtr.Zero;
        }
        #endregion


        #region Notify/Ignore
        public static void Notify(string key, Action<string, Type, dynamic> action)
        {
            var subscription = subscriptions.SingleOrDefault(s => s.Key == key);
            if (subscription != null)
            {
                subscription.ActionEvent += action;
            }
            else
            {
                subscription = new Subscription(key);
                subscription.ActionEvent += action;

                subscriptions.Add(subscription);
            }
        }

        public static void Ignore(string key, Action<string, Type, dynamic> action)
        {
            var subscription = subscriptions.SingleOrDefault(s => s.Key == key);
            if (subscription == null)
                return;

            subscription.ActionEvent -= action;

            if (subscription.ActionCount == 0)
                subscriptions.Remove(subscription);
        }
        #endregion

        #region Subscription
        private static IList<Subscription> subscriptions = new List<Subscription>();

        private class Subscription
        {
            private Interop.Vconf.VconfCallback _callback;

            internal string Key { get; }
            internal int ActionCount => (_actionEvent == null) ? 0 : _actionEvent.GetInvocationList().Count();

            private event Action<string, Type, dynamic> _actionEvent;
            internal event Action<string, Type, dynamic> ActionEvent
            {
                add
                {
                    AddEventHandler(value);
                }
                remove
                {
                    RemoveEventHandler(value);
                }
            }

            internal Subscription(string key)
            {
                Key = key;
                _callback = CallbackMethod;
                // CallbackMethod has to be stored in a field _callback. Otherwise it is garbage collected.
                // FailFast: A callback was made on a garbage collected delegate of type 'Tizen.Vconf!Interop+Vconf+VconfCallbackFn::Invoke'.
            }

            private void AddEventHandler(Action<string, Type, dynamic> action)
            {
                if (ActionCount == 0)
                {
                    int result = Interop.Vconf.VconfNotifyKeyChanged(Key, _callback, IntPtr.Zero);
                    if (result != 0)
                        throw new Exception($"Could not subscribe, vconf_notify_key_changed failed ({result}) for key: {Key}");
                }
                _actionEvent += action;
            }

            private void RemoveEventHandler(Action<string, Type, dynamic> action)
            {
                if (ActionCount == 1)
                {
                    int result = Interop.Vconf.VconfIgnoreKeyChanged(Key, _callback);
                    if (result != 0)
                        throw new Exception($"Could not unsubscribe, vconf_ignore_key_changed failed ({result}) for key: {Key}");
                }
                _actionEvent -= action;
            }

            private void CallbackMethod(IntPtr node, IntPtr userData)
            {
                try
                {
                    var keynode = Marshal.PtrToStructure<Interop.Vconf.KeyNode>(node);
                    var keyname = keynode.KeyName;
                    var keytype = keynode.Type;

                    void LogChange(dynamic value) => Log($"changed keyname: {keyname}, type: {keytype}, value: {value}");

                    switch (keytype)
                    {
                        case Interop.Vconf.Type.VCONF_TYPE_INT:
                            int ivalue = keynode.ValueInt;
                            LogChange(ivalue);
                            _actionEvent?.Invoke(Key, typeof(int), ivalue);
                            break;

                        case Interop.Vconf.Type.VCONF_TYPE_STRING:
                            string svalue = Marshal.PtrToStringAnsi(keynode.ValueString);
                            LogChange(svalue);
                            _actionEvent?.Invoke(Key, typeof(string), svalue);
                            break;

                        case Interop.Vconf.Type.VCONF_TYPE_BOOL:
                            bool bvalue = !(keynode.ValueBool == 0);
                            LogChange(bvalue);
                            _actionEvent?.Invoke(Key, typeof(bool), bvalue);
                            break;

                        case Interop.Vconf.Type.VCONF_TYPE_DOUBLE:
                            double dvalue = keynode.ValueDouble;
                            LogChange(dvalue);
                            _actionEvent?.Invoke(Key, typeof(double), dvalue);
                            break;
                    }
                }
                catch (Exception exc)
                {
                    Log($"{exc}");
                }
            }
        }
        #endregion
    }
}