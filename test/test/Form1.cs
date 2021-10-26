using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            AngoraDynamics di = new AngoraDynamics();
            //Cargando el menu dinamico desde un archivo json
            di.Load("menu.json", menuStrip1);
        }
    }

    public class AngoraDynamics
    {
        public void Load(string json, MenuStrip menu)
        {
            StreamReader reader = new StreamReader(json);
            string jsonText = reader.ReadToEnd();
            reader.Close();
            JObject obj = JObject.Parse(jsonText);
            getAllProperties(json, obj, menu);
        }

        private void getAllProperties(string jsonPath, JToken children, MenuStrip menu)
        {
            MenuStrip mainNode = menu;
            mainNode.Name = Path.GetFileNameWithoutExtension(jsonPath);

            foreach (JToken doc in children.Children())
            {
                var property = doc as JProperty;
                if (property != null)
                {
                    //############################################################
                    //######### OBTIENE UN OBJETO VACIO DEL DOCUMENTO ############
                    //############################################################
                    if (property.Value.ToString() == "Menu")
                    {
                        ToolStripMenuItem menuItem = new ToolStripMenuItem(property.Name);
                        menuItem.Name = property.Name + "MenuItem";
                        menuItem.Tag = property.Name;
                        mainNode.Items.AddRange(new ToolStripMenuItem[] { menuItem });
                    }

                    //###################################################################
                    //######### OBTIENE UN OBJETO CON DROPDOWN DEL DOCUMENTO ############
                    //###################################################################
                    if (property.Value.Type == JTokenType.Object)
                    {
                        //AÑADIENDO AL MENU STRIP
                        ToolStripMenuItem menuItem = new ToolStripMenuItem(property.Name);
                        menuItem.Name = property.Name + "MenuItem";
                        menuItem.Tag = property.Name;

                        //CARGANDO HIJOS
                        foreach (JToken item in property.Value.Children())
                        {
                            var property2 = item as JProperty;

                            if (property2.Value.Type == JTokenType.String)
                            {
                                //###############################################################################
                                //###### CARGANDO UN OBJETO DENTRO DE OTRO USANDO VALORES DISTINTOS #############
                                //###############################################################################
                                if (property2.Value.ToString() == "Options")
                                {
                                    ToolStripMenuItem menuItem2 = new ToolStripMenuItem(property2.Name);
                                    menuItem2.Name = property2.Name + "MenuItem";
                                    menuItem2.Tag = property2.Name;

                                    foreach (JToken item2 in property.Value.Children())
                                    {
                                        var property3 = item2 as JProperty;

                                        if (property3.Name == "displayFile")
                                        {
                                            string url = property3.Value.ToString().Replace("Url:", "").Trim();
                                            string[] lines = File.ReadAllLines(url);
                                            foreach (string l in lines)
                                            {
                                                ToolStripMenuItem display = new ToolStripMenuItem(l);
                                                display.Name = l + "MenuItem";
                                                display.Tag = l;
                                                menuItem2.DropDownItems.Add(display);
                                            }
                                        }

                                        if (property3.Value.ToString() == "Display")
                                        {
                                            ToolStripMenuItem display = new ToolStripMenuItem(property3.Name);
                                            display.Name = property3.Name + "MenuItem";
                                            display.Tag = property3.Name;
                                            menuItem2.DropDownItems.Add(display);
                                        }
                                    }
                                    menuItem.DropDownItems.Add(menuItem2);
                                }
                            }
                            //###################################################
                            //####### CARGANDO OBJETOS BASICOS SI HIJOS #########
                            //###################################################
                            if (property2.Value.Type == JTokenType.String)
                            {
                                if (property2.Value.ToString() == "OnClick")
                                {
                                    ToolStripMenuItem onClick = new ToolStripMenuItem(property2.Name);
                                    onClick.Name = property2.Name + "MenuItem";
                                    onClick.Tag = property2.Name;

                                    menuItem.DropDownItems.Add(onClick);
                                }

                                if (property2.Value.ToString() == "Separator")
                                {
                                    ToolStripSeparator sep = new ToolStripSeparator();
                                    menuItem.DropDownItems.Add(sep);
                                }

                            }
                        }
                        //AGREGO EL OBJETO AL MENU STRIP
                        mainNode.Items.AddRange(new ToolStripMenuItem[] { menuItem });
                    }
                }
                getAllProperties(jsonPath, doc, menu);
            }

        }
    }
}
