using FusionClient.API.QM;
using FusionClient.API.SM;
using System.Collections.Generic;

namespace FusionClient.API
{
    public static class BlazesAPI
    {
        public const string Identifier = "FusionUwU";

        public static List<QMSingleButton> allQMSingleButtons = new();
        public static List<QMNestedButton> allQMNestedButtons = new();
        public static List<QMToggleButton> allQMToggleButtons = new();
        public static List<QMTabButton> allQMTabButtons = new();
        public static List<QMInfo> allQMInfos = new();
        public static List<QMSlider> allQMSliders = new();
        public static List<QMLabel> allQMLabels = new();
        public static List<SMButton> allSMButtons = new();
        public static List<SMList> allSMLists = new();
        public static List<SMText> allSMTexts = new();
        public static List<SMPopup> allSMPopups = new();
    }
}
