using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Events;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using System.Drawing;
using System.Windows.Media;

namespace Ribbon
{

    // Knap class. For hvert script/funktion oprettes en Knap med data om funktionen/scriptet.
    public class ButtonDefinition
    {
        public string internalButtonName;
        public string visibleButtonName;
        public string namespaceName;
        public string className;
        public string shortTooltip;
        public string longTooltip;
        public string panelName;
        public string iconFileName;

        /// <summary>
        /// Knap Constructor. Opretter Knap ved angivelse af tilhørende data
        /// </summary>
        /// <param name="interntKnapnavn"> Fremgår ikke i Revit </param>
        /// <param name="synligtKnapNavn"     >Navn, som ses under selve knappen i Revit</param>
        /// <param name="namespaceNavn">Navn på namespace i tilhørende dll-fil</param>
        /// <param name="className"     >Navn på den class, som skal køres</param>
        /// <param name="kortTooltip"  >Kort tooltip, der vises med det samme i Revit ved mouseover</param>
        /// <param name="langtTooltip" >Langt tooltip, der folder sig ned efter nogle sekunders mouseover</param>
        /// <param name="panelNavn"    >Navn på det panel, knappen hører under</param>
        /// <param name="ikonFilnavn"  >// Knap-ikon (png eller ico). 32x32px for store ikoner, 16x16px for små ikoner</param>
        public ButtonDefinition(string interntKnapnavn, string synligtKnapNavn, string namespaceNavn, string className, string kortTooltip, string langtTooltip, string panelNavn, string ikonFilnavn)
        {
            this.internalButtonName = interntKnapnavn; // Fremgår ikke i Revit
            this.visibleButtonName  = synligtKnapNavn; // Navn, som ses under selve knappen i Revit
            this.namespaceName      = namespaceNavn;   // Navn på namespace i tilhørende dll-fil
            this.className          = className;       // Navn på den class, som skal køres
            this.shortTooltip       = kortTooltip;     // Kort tooltip, der vises med det samme i Revit ved mouseover
            this.longTooltip        = langtTooltip;    // Langt tooltip, der folder sig ned efter nogle sekunders mouseover
            this.panelName          = panelNavn;       // Navn på det panel, knappen hører under
            this.iconFileName       = ikonFilnavn;     // Knap-ikon (png eller ico). 32x32px for store ikoner, 16x16px for små ikoner
        }
    }

    class Ribbons : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            // Laver RibbonTab 
            string RibbonNavn = "Design Platform";
            application.CreateRibbonTab(RibbonNavn);

            string versionNumber = application.ControlledApplication.VersionNumber;


            // Sti til denne dll (sti til dll med dét script, en knap skal køre):
            string thisAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            ////////////////
            //// PANELS ////
            // Laver ribbon panel kaldt "Sheets" under fanen "Rubow"
            Dictionary<string, RibbonPanel> knapPanel = new Dictionary<string, RibbonPanel>();

            // Navne på panels
            List<string> panelNames = new List<string>(new string[]{
                "Import"
            });

            // Opretter RibbonPanel for hvert navn:
            foreach (string panelName in panelNames)
            {
                knapPanel.Add(panelName, application.CreateRibbonPanel(RibbonNavn, panelName));
            }
            
            // //////////
            // KNAPPER //
            // Liste med knapper for alle scripts. Hver knap angiver et script
            List<ButtonDefinition> scriptOversigt = new List<ButtonDefinition>
            {
                new ButtonDefinition("ImportWalls", "Import Project", "DesignPlatform", "ImportWalls", "Import building elements from Design Platform project.","", "Import", "importWalls_icon.png"),

            };
            
            // Opretter knapper på Revit ribbon for hvert script i scriptOversigt
            foreach (ButtonDefinition script in scriptOversigt)
            {
                Uri bt_ikon = new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\"+ versionNumber + @"\DesignPlatform\" + script.iconFileName);

                PushButtonData bt = new PushButtonData(script.internalButtonName, script.visibleButtonName, thisAssemblyPath, script.namespaceName + "." + script.className)
                {
                    AvailabilityClassName   = script.namespaceName + ".Availability",
                    ToolTip                 = script.shortTooltip,
                    LongDescription         = script.longTooltip,
                    LargeImage              = new BitmapImage(bt_ikon)
                };

                knapPanel[script.panelName].AddItem(bt);
            }

            return Result.Succeeded;
        }


        // Lukker ribbon når Revit lukkes
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }


    }
}
