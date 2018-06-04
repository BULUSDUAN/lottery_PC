namespace KaSon.FrameWork.Helper
{
    //using Microsoft.Win32;
    //using System;

    //public class RegisterHelper
    //{
    //    private RegDomain _domain;
    //    private string _regeditkey;
    //    private string _subkey;

    //    public RegisterHelper()
    //    {
    //        this._subkey = @"software\";
    //        this._domain = RegDomain.LocalMachine;
    //    }

    //    public RegisterHelper(string subKey, RegDomain regDomain)
    //    {
    //        this._subkey = subKey;
    //        this._domain = regDomain;
    //    }

    //    public virtual void CreateSubKey()
    //    {
    //        if ((this._subkey != string.Empty) && (this._subkey != null))
    //        {
    //            RegistryKey regDomain = this.GetRegDomain(this._domain);
    //            if (!this.IsSubKeyExist())
    //            {
    //                RegistryKey key2 = regDomain.CreateSubKey(this._subkey);
    //            }
    //            regDomain.Close();
    //        }
    //    }

    //    public virtual void CreateSubKey(RegDomain regDomain)
    //    {
    //        if ((this._subkey != string.Empty) && (this._subkey != null))
    //        {
    //            RegistryKey key = this.GetRegDomain(regDomain);
    //            if (!this.IsSubKeyExist(regDomain))
    //            {
    //                RegistryKey key2 = key.CreateSubKey(this._subkey);
    //            }
    //            key.Close();
    //        }
    //    }

    //    public virtual void CreateSubKey(string subKey)
    //    {
    //        if ((subKey != string.Empty) && (subKey != null))
    //        {
    //            RegistryKey regDomain = this.GetRegDomain(this._domain);
    //            if (!this.IsSubKeyExist(subKey))
    //            {
    //                RegistryKey key2 = regDomain.CreateSubKey(subKey);
    //            }
    //            regDomain.Close();
    //        }
    //    }

    //    public virtual void CreateSubKey(string subKey, RegDomain regDomain)
    //    {
    //        if ((subKey != string.Empty) && (subKey != null))
    //        {
    //            RegistryKey key = this.GetRegDomain(regDomain);
    //            if (!this.IsSubKeyExist(subKey, regDomain))
    //            {
    //                RegistryKey key2 = key.CreateSubKey(subKey);
    //            }
    //            key.Close();
    //        }
    //    }

    //    public virtual bool DeleteRegeditKey()
    //    {
    //        bool flag = false;
    //        if ((this._regeditkey == string.Empty) || (this._regeditkey == null))
    //        {
    //            return false;
    //        }
    //        if (this.IsRegeditKeyExist(this._regeditkey))
    //        {
    //            RegistryKey key = this.OpenSubKey(true);
    //            if (key != null)
    //            {
    //                try
    //                {
    //                    key.DeleteValue(this._regeditkey);
    //                    flag = true;
    //                }
    //                catch
    //                {
    //                    flag = false;
    //                }
    //                finally
    //                {
    //                    key.Close();
    //                }
    //            }
    //        }
    //        return flag;
    //    }

    //    public virtual bool DeleteRegeditKey(string name)
    //    {
    //        bool flag = false;
    //        if ((name == string.Empty) || (name == null))
    //        {
    //            return false;
    //        }
    //        if (this.IsRegeditKeyExist(name))
    //        {
    //            RegistryKey key = this.OpenSubKey(true);
    //            if (key != null)
    //            {
    //                try
    //                {
    //                    key.DeleteValue(name);
    //                    flag = true;
    //                }
    //                catch
    //                {
    //                    flag = false;
    //                }
    //                finally
    //                {
    //                    key.Close();
    //                }
    //            }
    //        }
    //        return flag;
    //    }

    //    public virtual bool DeleteRegeditKey(string name, string subKey)
    //    {
    //        bool flag = false;
    //        if ((((name == string.Empty) || (name == null)) || (subKey == string.Empty)) || (subKey == null))
    //        {
    //            return false;
    //        }
    //        if (this.IsRegeditKeyExist(name))
    //        {
    //            RegistryKey key = this.OpenSubKey(subKey, true);
    //            if (key != null)
    //            {
    //                try
    //                {
    //                    key.DeleteValue(name);
    //                    flag = true;
    //                }
    //                catch
    //                {
    //                    flag = false;
    //                }
    //                finally
    //                {
    //                    key.Close();
    //                }
    //            }
    //        }
    //        return flag;
    //    }

    //    public virtual bool DeleteRegeditKey(string name, string subKey, RegDomain regDomain)
    //    {
    //        bool flag = false;
    //        if ((((name == string.Empty) || (name == null)) || (subKey == string.Empty)) || (subKey == null))
    //        {
    //            return false;
    //        }
    //        if (this.IsRegeditKeyExist(name))
    //        {
    //            RegistryKey key = this.OpenSubKey(subKey, regDomain, true);
    //            if (key != null)
    //            {
    //                try
    //                {
    //                    key.DeleteValue(name);
    //                    flag = true;
    //                }
    //                catch
    //                {
    //                    flag = false;
    //                }
    //                finally
    //                {
    //                    key.Close();
    //                }
    //            }
    //        }
    //        return flag;
    //    }

    //    public virtual bool DeleteSubKey()
    //    {
    //        bool flag = false;
    //        if ((this._subkey == string.Empty) || (this._subkey == null))
    //        {
    //            return false;
    //        }
    //        RegistryKey regDomain = this.GetRegDomain(this._domain);
    //        if (this.IsSubKeyExist())
    //        {
    //            try
    //            {
    //                regDomain.DeleteSubKey(this._subkey);
    //                flag = true;
    //            }
    //            catch
    //            {
    //                flag = false;
    //            }
    //        }
    //        regDomain.Close();
    //        return flag;
    //    }

    //    public virtual bool DeleteSubKey(string subKey)
    //    {
    //        bool flag = false;
    //        if ((subKey == string.Empty) || (subKey == null))
    //        {
    //            return false;
    //        }
    //        RegistryKey regDomain = this.GetRegDomain(this._domain);
    //        if (this.IsSubKeyExist())
    //        {
    //            try
    //            {
    //                regDomain.DeleteSubKey(subKey);
    //                flag = true;
    //            }
    //            catch
    //            {
    //                flag = false;
    //            }
    //        }
    //        regDomain.Close();
    //        return flag;
    //    }

    //    public virtual bool DeleteSubKey(string subKey, RegDomain regDomain)
    //    {
    //        bool flag = false;
    //        if ((subKey == string.Empty) || (subKey == null))
    //        {
    //            return false;
    //        }
    //        RegistryKey key = this.GetRegDomain(regDomain);
    //        if (this.IsSubKeyExist(subKey, regDomain))
    //        {
    //            try
    //            {
    //                key.DeleteSubKey(subKey);
    //                flag = true;
    //            }
    //            catch
    //            {
    //                flag = false;
    //            }
    //        }
    //        key.Close();
    //        return flag;
    //    }

    //    protected RegistryKey GetRegDomain(RegDomain regDomain)
    //    {
    //        switch (regDomain)
    //        {
    //            case RegDomain.ClassesRoot:
    //                return Registry.ClassesRoot;

    //            case RegDomain.CurrentUser:
    //                return Registry.CurrentUser;

    //            case RegDomain.LocalMachine:
    //                return Registry.LocalMachine;

    //            case RegDomain.User:
    //                return Registry.Users;

    //            case RegDomain.CurrentConfig:
    //                return Registry.CurrentConfig;

    //            case RegDomain.PerformanceData:
    //                return Registry.PerformanceData;
    //        }
    //        return Registry.LocalMachine;
    //    }

    //    protected RegistryValueKind GetRegValueKind(RegValueKind regValueKind)
    //    {
    //        switch (regValueKind)
    //        {
    //            case RegValueKind.Unknown:
    //                return RegistryValueKind.Unknown;

    //            case RegValueKind.String:
    //                return RegistryValueKind.String;

    //            case RegValueKind.ExpandString:
    //                return RegistryValueKind.ExpandString;

    //            case RegValueKind.Binary:
    //                return RegistryValueKind.Binary;

    //            case RegValueKind.DWord:
    //                return RegistryValueKind.DWord;

    //            case RegValueKind.MultiString:
    //                return RegistryValueKind.MultiString;

    //            case RegValueKind.QWord:
    //                return RegistryValueKind.QWord;
    //        }
    //        return RegistryValueKind.String;
    //    }

    //    public virtual bool IsRegeditKeyExist()
    //    {
    //        bool flag = false;
    //        if ((this._regeditkey == string.Empty) || (this._regeditkey == null))
    //        {
    //            return false;
    //        }
    //        if (this.IsSubKeyExist())
    //        {
    //            RegistryKey key = this.OpenSubKey();
    //            string[] valueNames = key.GetValueNames();
    //            foreach (string str in valueNames)
    //            {
    //                if (string.Compare(str, this._regeditkey, true) == 0)
    //                {
    //                    flag = true;
    //                    break;
    //                }
    //            }
    //            key.Close();
    //        }
    //        return flag;
    //    }

    //    public virtual bool IsRegeditKeyExist(string name)
    //    {
    //        bool flag = false;
    //        if ((name == string.Empty) || (name == null))
    //        {
    //            return false;
    //        }
    //        if (this.IsSubKeyExist())
    //        {
    //            RegistryKey key = this.OpenSubKey();
    //            string[] valueNames = key.GetValueNames();
    //            foreach (string str in valueNames)
    //            {
    //                if (string.Compare(str, name, true) == 0)
    //                {
    //                    flag = true;
    //                    break;
    //                }
    //            }
    //            key.Close();
    //        }
    //        return flag;
    //    }

    //    public virtual bool IsRegeditKeyExist(string name, string subKey)
    //    {
    //        bool flag = false;
    //        if ((name == string.Empty) || (name == null))
    //        {
    //            return false;
    //        }
    //        if (this.IsSubKeyExist())
    //        {
    //            RegistryKey key = this.OpenSubKey(subKey);
    //            string[] valueNames = key.GetValueNames();
    //            foreach (string str in valueNames)
    //            {
    //                if (string.Compare(str, name, true) == 0)
    //                {
    //                    flag = true;
    //                    break;
    //                }
    //            }
    //            key.Close();
    //        }
    //        return flag;
    //    }

    //    public virtual bool IsRegeditKeyExist(string name, string subKey, RegDomain regDomain)
    //    {
    //        bool flag = false;
    //        if ((name == string.Empty) || (name == null))
    //        {
    //            return false;
    //        }
    //        if (this.IsSubKeyExist())
    //        {
    //            RegistryKey key = this.OpenSubKey(subKey, regDomain);
    //            string[] valueNames = key.GetValueNames();
    //            foreach (string str in valueNames)
    //            {
    //                if (string.Compare(str, name, true) == 0)
    //                {
    //                    flag = true;
    //                    break;
    //                }
    //            }
    //            key.Close();
    //        }
    //        return flag;
    //    }

    //    public virtual bool IsSubKeyExist()
    //    {
    //        if ((this._subkey == string.Empty) || (this._subkey == null))
    //        {
    //            return false;
    //        }
    //        if (this.OpenSubKey(this._subkey, this._domain) == null)
    //        {
    //            return false;
    //        }
    //        return true;
    //    }

    //    public virtual bool IsSubKeyExist(RegDomain regDomain)
    //    {
    //        if ((this._subkey == string.Empty) || (this._subkey == null))
    //        {
    //            return false;
    //        }
    //        if (this.OpenSubKey(this._subkey, regDomain) == null)
    //        {
    //            return false;
    //        }
    //        return true;
    //    }

    //    public virtual bool IsSubKeyExist(string subKey)
    //    {
    //        if ((subKey == string.Empty) || (subKey == null))
    //        {
    //            return false;
    //        }
    //        if (this.OpenSubKey(subKey) == null)
    //        {
    //            return false;
    //        }
    //        return true;
    //    }

    //    public virtual bool IsSubKeyExist(string subKey, RegDomain regDomain)
    //    {
    //        if ((subKey == string.Empty) || (subKey == null))
    //        {
    //            return false;
    //        }
    //        if (this.OpenSubKey(subKey, regDomain) == null)
    //        {
    //            return false;
    //        }
    //        return true;
    //    }

    //    protected virtual RegistryKey OpenSubKey()
    //    {
    //        if ((this._subkey == string.Empty) || (this._subkey == null))
    //        {
    //            return null;
    //        }
    //        RegistryKey regDomain = this.GetRegDomain(this._domain);
    //        RegistryKey key2 = null;
    //        key2 = regDomain.OpenSubKey(this._subkey);
    //        regDomain.Close();
    //        return key2;
    //    }

    //    protected virtual RegistryKey OpenSubKey(bool writable)
    //    {
    //        if ((this._subkey == string.Empty) || (this._subkey == null))
    //        {
    //            return null;
    //        }
    //        RegistryKey regDomain = this.GetRegDomain(this._domain);
    //        RegistryKey key2 = null;
    //        key2 = regDomain.OpenSubKey(this._subkey, writable);
    //        regDomain.Close();
    //        return key2;
    //    }

    //    protected virtual RegistryKey OpenSubKey(string subKey)
    //    {
    //        if ((subKey == string.Empty) || (subKey == null))
    //        {
    //            return null;
    //        }
    //        RegistryKey regDomain = this.GetRegDomain(this._domain);
    //        RegistryKey key2 = null;
    //        key2 = regDomain.OpenSubKey(subKey);
    //        regDomain.Close();
    //        return key2;
    //    }

    //    protected virtual RegistryKey OpenSubKey(string subKey, RegDomain regDomain)
    //    {
    //        if ((subKey == string.Empty) || (subKey == null))
    //        {
    //            return null;
    //        }
    //        RegistryKey key = this.GetRegDomain(regDomain);
    //        RegistryKey key2 = null;
    //        key2 = key.OpenSubKey(subKey);
    //        key.Close();
    //        return key2;
    //    }

    //    protected virtual RegistryKey OpenSubKey(string subKey, bool writable)
    //    {
    //        if ((subKey == string.Empty) || (subKey == null))
    //        {
    //            return null;
    //        }
    //        RegistryKey regDomain = this.GetRegDomain(this._domain);
    //        RegistryKey key2 = null;
    //        key2 = regDomain.OpenSubKey(subKey, writable);
    //        regDomain.Close();
    //        return key2;
    //    }

    //    protected virtual RegistryKey OpenSubKey(string subKey, RegDomain regDomain, bool writable)
    //    {
    //        if ((subKey == string.Empty) || (subKey == null))
    //        {
    //            return null;
    //        }
    //        RegistryKey key = this.GetRegDomain(regDomain);
    //        RegistryKey key2 = null;
    //        key2 = key.OpenSubKey(subKey, writable);
    //        key.Close();
    //        return key2;
    //    }

    //    public virtual object ReadRegeditKey()
    //    {
    //        object obj2 = null;
    //        if ((this._regeditkey == string.Empty) || (this._regeditkey == null))
    //        {
    //            return null;
    //        }
    //        if (this.IsRegeditKeyExist(this._regeditkey))
    //        {
    //            RegistryKey key = this.OpenSubKey();
    //            if (key != null)
    //            {
    //                obj2 = key.GetValue(this._regeditkey);
    //            }
    //            key.Close();
    //        }
    //        return obj2;
    //    }

    //    public virtual object ReadRegeditKey(string name)
    //    {
    //        object obj2 = null;
    //        if ((name == string.Empty) || (name == null))
    //        {
    //            return null;
    //        }
    //        if (this.IsRegeditKeyExist(name))
    //        {
    //            RegistryKey key = this.OpenSubKey();
    //            if (key != null)
    //            {
    //                obj2 = key.GetValue(name);
    //            }
    //            key.Close();
    //        }
    //        return obj2;
    //    }

    //    public virtual object ReadRegeditKey(string name, string subKey)
    //    {
    //        object obj2 = null;
    //        if ((name == string.Empty) || (name == null))
    //        {
    //            return null;
    //        }
    //        if (this.IsRegeditKeyExist(name))
    //        {
    //            RegistryKey key = this.OpenSubKey(subKey);
    //            if (key != null)
    //            {
    //                obj2 = key.GetValue(name);
    //            }
    //            key.Close();
    //        }
    //        return obj2;
    //    }

    //    public virtual object ReadRegeditKey(string name, string subKey, RegDomain regDomain)
    //    {
    //        object obj2 = null;
    //        if ((name == string.Empty) || (name == null))
    //        {
    //            return null;
    //        }
    //        if (this.IsRegeditKeyExist(name))
    //        {
    //            RegistryKey key = this.OpenSubKey(subKey, regDomain);
    //            if (key != null)
    //            {
    //                obj2 = key.GetValue(name);
    //            }
    //            key.Close();
    //        }
    //        return obj2;
    //    }

    //    public virtual bool WriteRegeditKey(object content)
    //    {
    //        bool flag = false;
    //        if ((this._regeditkey == string.Empty) || (this._regeditkey == null))
    //        {
    //            return false;
    //        }
    //        if (!this.IsSubKeyExist(this._subkey))
    //        {
    //            this.CreateSubKey(this._subkey);
    //        }
    //        RegistryKey key = this.OpenSubKey(true);
    //        if (key == null)
    //        {
    //            return false;
    //        }
    //        try
    //        {
    //            key.SetValue(this._regeditkey, content);
    //            flag = true;
    //        }
    //        catch
    //        {
    //            flag = false;
    //        }
    //        finally
    //        {
    //            key.Close();
    //        }
    //        return flag;
    //    }

    //    public virtual bool WriteRegeditKey(string name, object content)
    //    {
    //        bool flag = false;
    //        if ((name == string.Empty) || (name == null))
    //        {
    //            return false;
    //        }
    //        if (!this.IsSubKeyExist(this._subkey))
    //        {
    //            this.CreateSubKey(this._subkey);
    //        }
    //        RegistryKey key = this.OpenSubKey(true);
    //        if (key == null)
    //        {
    //            return false;
    //        }
    //        try
    //        {
    //            key.SetValue(name, content);
    //            flag = true;
    //        }
    //        catch (Exception)
    //        {
    //            flag = false;
    //        }
    //        finally
    //        {
    //            key.Close();
    //        }
    //        return flag;
    //    }

    //    public virtual bool WriteRegeditKey(string name, object content, RegValueKind regValueKind)
    //    {
    //        bool flag = false;
    //        if ((name == string.Empty) || (name == null))
    //        {
    //            return false;
    //        }
    //        if (!this.IsSubKeyExist(this._subkey))
    //        {
    //            this.CreateSubKey(this._subkey);
    //        }
    //        RegistryKey key = this.OpenSubKey(true);
    //        if (key == null)
    //        {
    //            return false;
    //        }
    //        try
    //        {
    //            key.SetValue(name, content, this.GetRegValueKind(regValueKind));
    //            flag = true;
    //        }
    //        catch
    //        {
    //            flag = false;
    //        }
    //        finally
    //        {
    //            key.Close();
    //        }
    //        return flag;
    //    }

    //    public RegDomain Domain
    //    {
    //        set
    //        {
    //            this._domain = value;
    //        }
    //    }

    //    public string RegeditKey
    //    {
    //        set
    //        {
    //            this._regeditkey = value;
    //        }
    //    }

    //    public string SubKey
    //    {
    //        set
    //        {
    //            this._subkey = value;
    //        }
    //    }
   // }
}

