using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ComEx
{        
    public class GlobalConfig
    {
        private static GlobalConfig myGlobalConfig  = new GlobalConfig();

        private GlobalConfig() {}

        public readonly string dateFormat = "yyyy/MM/dd";

        public static GlobalConfig get(){
            return myGlobalConfig;
        }

        public int AppAnoFiltro {            
            get {
                int value = DateTime.Now.Year;

                if (HttpContext.Current.Session["AppAnoFiltro"] != null) {
                    value = (int)HttpContext.Current.Session["AppAnoFiltro"];
                }

                HttpContext.Current.Session["AppAnoFiltro"] = value;
                return value;
            }
            set {
                HttpContext.Current.Session["AppAnoFiltro"] = value;
            }
        }

        public bool ChkFiltroPorAno
        {
            get
            {
                bool value = true;

                if (HttpContext.Current.Session["ChkFiltroPorAno"] != null) {
                    value = (bool)HttpContext.Current.Session["ChkFiltroPorAno"];
                }

                HttpContext.Current.Session["ChkFiltroPorAno"] = value;
                return value;
            }
            set
            {
                HttpContext.Current.Session["ChkFiltroPorAno"] = value;
            }
        }
    }
}