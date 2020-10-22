﻿using PBE.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace PBE.Actions
{
    internal class ExportDoc : FSConsole
    {
        public String Package { get; private set; }
        public String Version { get; private set; }
        public String Iso { get; private set; }
        public bool ExportDBTables { get; private set; }
        public String Dir { get; private set; }
        protected virtual string ExportDirParameter { get { return "ExportDir"; } }

        public ExportDoc(XElement xe, ExecutableContainer container, int indent)
            : base(xe, container, indent)
        {
            this.Package = container.ParseParameters(xe.Attribute("Package").Value);
            this.Version = container.ParseParameters(xe.Attribute("Version").Value);
            this.Iso = container.ParseParameters(xe.Attribute("Iso").Value);
            this.ExportDBTables = (string)xe.Attribute("ExportDBTables") == "1";
            var xaDir = xe.Attribute("Dir");
            if (xaDir != null)
            {
                Dir = container.ParseParameters(xaDir.Value);
            }
            else
            {
                Dir = container.ParseParameters("{" + this.ExportDirParameter + "}");
            }

            string folder = Path.Combine(this.Dir, container.CreateExportFileName(this.Package, this.Version) + "_Help_" + this.Iso);

            var args = new List<string>()
            {
                @"\DOCUMENTATION",
                @"\PACKAGE", this.Package,
                @"\VERSION", this.Version,
                @"\ISO", this.Iso,
                @"\OUTPUT", folder
            };

            if (ExportDBTables)
            {
                args.Add(@"\ExportDBTables");
            }

            this.Arguments = FSUtils.EscapeCommandLineArgs(args)
                // ggf. manuelle "Args" aus dem Basis-Konstruktor übernehmen
                + (!string.IsNullOrEmpty(this.Arguments) ? " " + this.Arguments : "");
        }

        public override void ExecuteAction()
        {
            base.ExecuteAction();

            if (this.ExitCode1 == -1)
            {
                this.TaskFailed = true;
            }
        }

        public override string Description
        {
            get
            {
                return "FS " + this.FSVer.ToString(2) + " " + this.Rep + " ExportDoc " + this.Package + " - " + this.Version + " - " + this.Iso
                     + (this.ExitCode1.HasValue ? (" (" + this.ExitCode1.Value + ")") : "");
            }
        }
    }
}