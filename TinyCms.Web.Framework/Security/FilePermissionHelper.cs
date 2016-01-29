using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using TinyCms.Core;

namespace TinyCms.Web.Framework.Security
{
    /// <summary>
    ///     File permission helper
    /// </summary>
    public static class FilePermissionHelper
    {
        /// <summary>
        ///     Check permissions
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="checkRead">Check read</param>
        /// <param name="checkWrite">Check write</param>
        /// <param name="checkModify">Check modify</param>
        /// <param name="checkDelete">Check delete</param>
        /// <returns>Result</returns>
        public static bool CheckPermissions(string path, bool checkRead, bool checkWrite, bool checkModify,
            bool checkDelete)
        {
            var flag = false;
            var flag2 = false;
            var flag3 = false;
            var flag4 = false;
            var flag5 = false;
            var flag6 = false;
            var flag7 = false;
            var flag8 = false;
            var current = WindowsIdentity.GetCurrent();
            AuthorizationRuleCollection rules;
            try
            {
                rules = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof (SecurityIdentifier));
            }
            catch
            {
                return true;
            }
            try
            {
                foreach (FileSystemAccessRule rule in rules)
                {
                    if (!current.User.Equals(rule.IdentityReference))
                    {
                        continue;
                    }
                    if (AccessControlType.Deny.Equals(rule.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule.FileSystemRights) == FileSystemRights.Delete)
                            flag4 = true;
                        if ((FileSystemRights.Modify & rule.FileSystemRights) == FileSystemRights.Modify)
                            flag3 = true;

                        if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                            flag = true;

                        if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                            flag2 = true;

                        continue;
                    }
                    if (AccessControlType.Allow.Equals(rule.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule.FileSystemRights) == FileSystemRights.Delete)
                        {
                            flag8 = true;
                        }
                        if ((FileSystemRights.Modify & rule.FileSystemRights) == FileSystemRights.Modify)
                        {
                            flag7 = true;
                        }
                        if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                        {
                            flag5 = true;
                        }
                        if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                        {
                            flag6 = true;
                        }
                    }
                }
                foreach (var reference in current.Groups)
                {
                    foreach (FileSystemAccessRule rule2 in rules)
                    {
                        if (!reference.Equals(rule2.IdentityReference))
                        {
                            continue;
                        }
                        if (AccessControlType.Deny.Equals(rule2.AccessControlType))
                        {
                            if ((FileSystemRights.Delete & rule2.FileSystemRights) == FileSystemRights.Delete)
                                flag4 = true;
                            if ((FileSystemRights.Modify & rule2.FileSystemRights) == FileSystemRights.Modify)
                                flag3 = true;
                            if ((FileSystemRights.Read & rule2.FileSystemRights) == FileSystemRights.Read)
                                flag = true;
                            if ((FileSystemRights.Write & rule2.FileSystemRights) == FileSystemRights.Write)
                                flag2 = true;
                            continue;
                        }
                        if (AccessControlType.Allow.Equals(rule2.AccessControlType))
                        {
                            if ((FileSystemRights.Delete & rule2.FileSystemRights) == FileSystemRights.Delete)
                                flag8 = true;
                            if ((FileSystemRights.Modify & rule2.FileSystemRights) == FileSystemRights.Modify)
                                flag7 = true;
                            if ((FileSystemRights.Read & rule2.FileSystemRights) == FileSystemRights.Read)
                                flag5 = true;
                            if ((FileSystemRights.Write & rule2.FileSystemRights) == FileSystemRights.Write)
                                flag6 = true;
                        }
                    }
                }
                var flag9 = !flag4 && flag8;
                var flag10 = !flag3 && flag7;
                var flag11 = !flag && flag5;
                var flag12 = !flag2 && flag6;
                var flag13 = true;
                if (checkRead)
                {
                    flag13 = flag13 && flag11;
                }
                if (checkWrite)
                {
                    flag13 = flag13 && flag12;
                }
                if (checkModify)
                {
                    flag13 = flag13 && flag10;
                }
                if (checkDelete)
                {
                    flag13 = flag13 && flag9;
                }
                return flag13;
            }
            catch (IOException)
            {
            }
            return false;
        }

        /// <summary>
        ///     Gets a list of directories (physical paths) which require write permission
        /// </summary>
        /// <param name="webHelper">Web helper</param>
        /// <returns>Result</returns>
        public static IEnumerable<string> GetDirectoriesWrite(IWebHelper webHelper)
        {
            var rootDir = webHelper.MapPath("~/");
            var dirsToCheck = new List<string>();
            //dirsToCheck.Add(rootDir);
            dirsToCheck.Add(Path.Combine(rootDir, "App_Data"));
            dirsToCheck.Add(Path.Combine(rootDir, "bin"));
            dirsToCheck.Add(Path.Combine(rootDir, "content"));
            dirsToCheck.Add(Path.Combine(rootDir, "content\\images"));
            dirsToCheck.Add(Path.Combine(rootDir, "content\\images\\thumbs"));
            dirsToCheck.Add(Path.Combine(rootDir, "content\\images\\uploaded"));
            dirsToCheck.Add(Path.Combine(rootDir, "content\\files\\exportimport"));
            dirsToCheck.Add(Path.Combine(rootDir, "plugins"));
            dirsToCheck.Add(Path.Combine(rootDir, "plugins\\bin"));
            return dirsToCheck;
        }

        /// <summary>
        ///     Gets a list of files (physical paths) which require write permission
        /// </summary>
        /// <param name="webHelper">Web helper</param>
        /// <returns>Result</returns>
        public static IEnumerable<string> GetFilesWrite(IWebHelper webHelper)
        {
            var rootDir = webHelper.MapPath("~/");
            var filesToCheck = new List<string>();
            filesToCheck.Add(Path.Combine(rootDir, "Global.asax"));
            filesToCheck.Add(Path.Combine(rootDir, "web.config"));
            filesToCheck.Add(Path.Combine(rootDir, "App_Data\\InstalledPlugins.txt"));
            filesToCheck.Add(Path.Combine(rootDir, "App_Data\\Settings.txt"));
            return filesToCheck;
        }
    }
}