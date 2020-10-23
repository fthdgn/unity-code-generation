using System;
using System.IO;
using UnityEditor;

namespace CodeGenerator
{
    public abstract class BaseGenerator
    {
        protected abstract string FileName { get; }
        private string lastWrittenCode = "";
        private DateTime lastWriteDate = DateTime.MinValue;

        public void Start()
        {
            Update();
            EditorApplication.update += Update;
        }

        private void Update()
        {
            var file = new FileInfo($"{Common.GeneratedCodeRoot}{FileName}.cs");
            if (file.Exists && lastWriteDate.AddMinutes(1) > DateTime.Now) return; //Don't update too often
            string newCode;
            try
            {
                newCode = GenerateCode();
            }
            catch (Exception e)
            {
                newCode = e.ToString();
            }

            if (file.Exists && newCode == lastWrittenCode) return; //Nothing changed

            if (file.Exists && File.ReadAllText(file.FullName) == newCode) return; //Nothing changed

            Directory.CreateDirectory(Common.GeneratedCodeRoot);
            file.Delete();
            file.Create().Dispose();
            var streamWriter = file.AppendText();
            streamWriter.WriteLine("// ReSharper disable All");
            streamWriter.Write(newCode);
            streamWriter.Close();
            AssetDatabase.Refresh();
            lastWrittenCode = newCode;
            lastWriteDate = DateTime.Now;
        }

        protected abstract string GenerateCode();
    }
}
