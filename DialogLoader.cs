using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DComposer
{
    public static class DialogLoader
    {

        public static void Save(DialogPage page, string filename)
        {
            dialogs dlgs = new dialogs();
            translate_page(page, dlgs);

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (StreamWriter sw = new StreamWriter(filename))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, dlgs);
                }
            }

        }

        private static void translate_page(DialogPage page, dialogs dlgs)
        {
            if (page == null) return;

            dialog dlg = new dialog();
            dlgs.items.Add(dlg);
            dlg.name = page.Label;
            dlg.starting_text = page.DisplayText;
            foreach(var option in page.Options)
            {
                if (String.IsNullOrWhiteSpace(option.DisplayText)) continue;

                choice ch = new choice();
                ch.text = option.DisplayText;
                ch.tree = option.Label;

                if (!String.IsNullOrWhiteSpace(option.Command))
                    ch.target = option.Command;
                else
                    ch.target = "self";


                foreach(var condition in option.Conditions)
                {
                    if ( String.IsNullOrEmpty(condition.Skill) )
                    {
                        continue;
                    }

                    condition con = new condition();
                    con.name = condition.Skill;
                    con.op = condition.TypeString;
                    con.value = condition.Value;
                    ch.conditions.Add(con);
                }

                foreach (var modifier in option.Modifiers)
                {
                    if (String.IsNullOrEmpty(modifier.Skill))
                    {
                        continue;
                    }

                    modifiers mod = new modifiers();
                    mod.name = modifier.Skill;
                    mod.op = modifier.TypeString;
                    mod.value = modifier.Value;
                    ch.modifiers.Add(mod);
                }
                dlg.choices.Add(ch);

                translate_page(option.Target, dlgs);
            }
        }

        private static DialogPage translate_dialog(dialog dlg)
        {
            DialogPage dp = new DialogPage(null);
            dp.Label = dlg.name;
            dp.DisplayText = dlg.starting_text;
            dp.Options = new ObservableCollection<DialogOption>();
            foreach( var choice in dlg.choices)
            {
                DialogOption option = new DialogOption();
                option.LabelBinding = choice.tree;
                option.DisplayText = choice.text;
                if (choice.target != "self")
                    option.Command = choice.target;

                for(int i = 0; i < choice.conditions.Count; i++)
                {
                    option.Conditions[i].Skill = choice.conditions[i].name;
                    option.Conditions[i].Value = choice.conditions[i].value;

                    switch( choice.conditions[i].op)
                    {
                        case "=":
                            option.Conditions[i].Type = ConditionTypes.Equal;
                            break;
                        case "!":
                            option.Conditions[i].Type = ConditionTypes.NotEqual;
                            break;
                        case ">":
                            option.Conditions[i].Type = ConditionTypes.GreaterThan;
                            break;
                        case "<":
                            option.Conditions[i].Type = ConditionTypes.LessThan;
                            break;
                    }                        
                }

                for (int i = 0; i < choice.modifiers.Count; i++)
                {
                    option.Modifiers[i].Skill = choice.modifiers[i].name;
                    option.Modifiers[i].Value = choice.modifiers[i].value;

                    switch (choice.modifiers[i].op)
                    {
                        case "=":
                            option.Modifiers[i].Type = ModifierTypes.Equal;
                            break;
                        case "+":
                            option.Modifiers[i].Type = ModifierTypes.Add;
                            break;
                        case "-":
                            option.Modifiers[i].Type = ModifierTypes.Subtract;
                            break;
                        case "*":
                            option.Modifiers[i].Type = ModifierTypes.Multiply;
                            break;
                        case "/":
                            option.Modifiers[i].Type = ModifierTypes.Divide;
                            break;
                    }
                }

                dp.Options.Add(option);
            }

            return dp;
       }

        public static DialogPage Load(string filename)
        {
            dialogs dlgs = null;

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (StreamReader sr = new StreamReader(filename))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    dlgs = serializer.Deserialize<dialogs>(reader);
                }
            }

            Dictionary<string, DialogPage> pages = new Dictionary<string, DialogPage>();
            foreach(var dlg in dlgs.items)
            {
                var page = translate_dialog(dlg);
                pages[page.Label] = page;
            }

            foreach(var page in pages.Values)
            {
                foreach(var option in page.Options)
                {
                    if (option.LabelBinding != null && pages.ContainsKey(option.LabelBinding))
                        option.Target = pages[option.LabelBinding];
                }
            }

            var rootPage = pages[dlgs.items[0].name];
            return rootPage;
        }
    }
    
    class dialogs
    {
        public List<dialog> items = new List<dialog>();
    }

    class dialog
    {
        public List<choice> choices = new List<choice>();
        public string name;
        public string starting_text;
    }

    class choice
    {
        public List<condition> conditions = new List<condition>();
        public List<modifiers> modifiers = new List<modifiers>();
        public string text;
        public string callback;
        public string target;
        public string tree;
    }

    class condition
    {
        public string name;
        public string op;
        public string value;
    }

    class modifiers
    {
        public string name;
        public string op;
        public string value;
    }
}
