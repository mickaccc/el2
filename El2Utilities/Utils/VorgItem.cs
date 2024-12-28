using El2Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace El2Core.Utils
{
    public class VorgItem(Vorgang vorgang)
    {
        public string Auftrag { get; } = vorgang.Aid;
        public string Vorgang { get; } = vorgang.Vnr.ToString("D4");
        public string? Kurztext { get; } = vorgang.Text;
        public string? Material { get; } = vorgang.AidNavigation.Material ?? vorgang.AidNavigation.DummyMat;
        public string? Bezeichnung { get; } = vorgang.AidNavigation.MaterialNavigation?.Bezeichng ??
            vorgang.AidNavigation.DummyMatNavigation?.Mattext;
        public Vorgang SourceVorgang = vorgang;
    }

}
