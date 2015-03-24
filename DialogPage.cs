using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DComposer
{

    public enum ModifierTypes
    {
        Equal = 0,
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public class ComboBoxModiferType
    {
        public ModifierTypes ValueConditionEnum { get; set; }
        public string ValueConditionString
        {
            get
            {
                if (ValueConditionEnum == ModifierTypes.Equal)
                    return "=";
                if (ValueConditionEnum == ModifierTypes.Add)
                    return "+";
                if (ValueConditionEnum == ModifierTypes.Subtract)
                    return "-";
                if (ValueConditionEnum == ModifierTypes.Multiply)
                    return "*";
                if (ValueConditionEnum == ModifierTypes.Divide)
                    return "/";

                return "";
            }
            set
            {
                if (value == "=")
                    ValueConditionEnum = ModifierTypes.Equal;
                if (value == "+")
                    ValueConditionEnum = ModifierTypes.Add;
                if (value == "-")
                    ValueConditionEnum = ModifierTypes.Subtract;
                if (value == "*")
                    ValueConditionEnum = ModifierTypes.Multiply;
                if (value == "/")
                    ValueConditionEnum = ModifierTypes.Divide;
            }
        }
    }

    public enum ConditionTypes
    {
        Equal = 0,
        NotEqual,
        LessThan,
        GreaterThan
    }

    public class ComboBoxConditionType
    {
        public ConditionTypes ValueConditionEnum { get; set; }
        public string ValueConditionString 
        {
            get
            {
                if (ValueConditionEnum == ConditionTypes.Equal)
                    return "=";
                if (ValueConditionEnum == ConditionTypes.NotEqual)
                    return "!";
                if (ValueConditionEnum == ConditionTypes.GreaterThan)
                    return ">";
                if (ValueConditionEnum == ConditionTypes.LessThan)
                    return "<";

                return "";
            }
            set
            {
                if (value == "=")
                    ValueConditionEnum = ConditionTypes.Equal;
                if (value == "!")
                    ValueConditionEnum = ConditionTypes.NotEqual;
                if (value == ">")
                    ValueConditionEnum = ConditionTypes.GreaterThan;
                if (value == "<")
                    ValueConditionEnum = ConditionTypes.LessThan;
            }
        }
    }


    public class DialogModifer : INotifyPropertyChanged
    {
        public string Skill { get; set; }
        public string Value { get; set; }

        public ModifierTypes Type { get; set; }

        public string TypeString 
        {
            get
            {
                if (Type == ModifierTypes.Equal)
                    return "=";
                if (Type == ModifierTypes.Add)
                    return "+";
                if (Type == ModifierTypes.Subtract)
                    return "-";
                if (Type == ModifierTypes.Multiply)
                    return "*";
                if (Type == ModifierTypes.Divide)
                    return "/";

                return "";
            }
        }

        public override string ToString()
        {
            return String.Format("{0}{1}{2}", Skill, TypeString, Value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class DialogCondition : INotifyPropertyChanged
    {
        public string Skill { get; set; }
        public string Value { get; set; }

        public ConditionTypes Type { get; set; }

        public string TypeString
        {
            get
            {
                if (Type == ConditionTypes.Equal)
                    return "=";
                if (Type == ConditionTypes.NotEqual)
                    return "!";
                if (Type == ConditionTypes.GreaterThan)
                    return ">";
                if (Type == ConditionTypes.LessThan)
                    return "<";

                return "";
            }
        }

        public override string ToString()
        {
            return String.Format("{0}{1}{2}", Skill, TypeString, Value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public interface DialogLabel
    {
         string Label {get; set; }
         ICommand onClick { get; set; }
         DataVertex Vertex { get; set; }
         DialogPage Parent { get; set; }


    }

    public class DialogOption : DialogLabel, INotifyPropertyChanged 
    {

        private static List<ComboBoxConditionType> mStaticConditions = new List<ComboBoxConditionType>();
        private static List<ComboBoxModiferType> mStaticModifers = new List<ComboBoxModiferType>();

        private string _Command;

        public List<ComboBoxConditionType> ComboboxConditionTypes 
        {
            get
            {
                return mStaticConditions;
            }
        }

        public List<ComboBoxModiferType> ComboboxModiferTypes
        {
            get { return mStaticModifers; }
        }

        public string LabelBinding { get; set; }
        public string Label 
        {
            get
            {
                if (Target != null)
                    return Target.Label;
                else
                    return LabelBinding;
            }
            set
            {
                if (Target != null)
                    Target.Label = value;
                else
                    LabelBinding = value;

            }
        }
        public ICommand onClick { get; set; }
        public DataVertex Vertex { get; set; }

        public DialogPage Target { get; set; }

        public List<DialogCondition> Conditions { get; set; }
        public List<DialogModifer> Modifiers { get; set; }

        public bool Enabled {get;set;}

        public DialogPage Parent { get; set; }

        public string Command 
        {
            get
            {
                return _Command;
            }
            set
            {
                _Command = value;
                if (Target != null)
                {
                    Target.isSubTreeCommand = false;
                    if (_Command != null)
                    {   
                        Target.isSubTreeCommand = _Command.Contains(".json");
                    }
                }
            }
        }
        public string DisplayText
        {
            get;
            set;
        }

        public string ConditionsText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[");

                var activeConditions = Conditions.Where(x => !String.IsNullOrWhiteSpace(x.Skill));

                sb.Append(String.Join(",", activeConditions));
                sb.Append("]");

                return sb.ToString();
            }
        }

        public string SettersText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[");

                var activeConditions = Modifiers.Where(x => !String.IsNullOrWhiteSpace(x.Skill));

                sb.Append(String.Join(",", activeConditions));
                sb.Append("]");

                return sb.ToString();
            }
        }

        static DialogOption()
        {
            mStaticConditions.Add(new ComboBoxConditionType() { ValueConditionEnum = ConditionTypes.Equal });
            mStaticConditions.Add(new ComboBoxConditionType() { ValueConditionEnum = ConditionTypes.NotEqual });
            mStaticConditions.Add(new ComboBoxConditionType() { ValueConditionEnum = ConditionTypes.GreaterThan });
            mStaticConditions.Add(new ComboBoxConditionType() { ValueConditionEnum = ConditionTypes.LessThan });

            mStaticModifers.Add(new ComboBoxModiferType() { ValueConditionEnum = ModifierTypes.Equal });
            mStaticModifers.Add(new ComboBoxModiferType() { ValueConditionEnum = ModifierTypes.Add });
            mStaticModifers.Add(new ComboBoxModiferType() { ValueConditionEnum = ModifierTypes.Subtract });
            mStaticModifers.Add(new ComboBoxModiferType() { ValueConditionEnum = ModifierTypes.Multiply });
            mStaticModifers.Add(new ComboBoxModiferType() { ValueConditionEnum = ModifierTypes.Divide });
        }
        public DialogOption()
        {
            Conditions = new List<DialogCondition>();
            for (int i = 0; i < 10; i++)
                Conditions.Add(new DialogCondition());

            Modifiers = new List<DialogModifer>();
            for (int i = 0; i < 10; i++)
                Modifiers.Add(new DialogModifer());

        }


        public void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void RefreshBinding()
        {
            OnPropertyChanged("DisplayText");
            OnPropertyChanged("ConditionsText");
            OnPropertyChanged("SettersText");
            OnPropertyChanged("Label");
        }

        public bool Contains(DialogPage page)
        {
            if (Target == null) return false;

            if (Target == page) return true;

            foreach(var option in Target.Options)
            {
                if (option.Contains(page)) return true;
            }

            return false;
        }
    }
    public class DialogPage : DialogLabel, INotifyPropertyChanged
    {
        private DialogWindow mOwner;
        public string Label { get; set; }
        public string DisplayText { get; set; }

        public ObservableCollection<DialogOption> Options {get;set;}      

        public ICommand onClick { get; set; }

        public DataVertex Vertex { get; set; }

        public DialogPage Parent 
        {
            get{return PageOwner;}
            set{PageOwner = value;}
        }

        public DialogOption OptionOwner
        {
            get;
            set;
        }

        public bool isSubTreeCommand
        {
            get;
            set;
        }

        public DialogPage PageOwner { get; set; }

        public DialogWindow Owner { get { return mOwner; } }
        public void setOwner(DialogWindow owner)
        {
            mOwner = owner;
        }
 
        public DialogPage(DialogWindow owner)
        {
            mOwner = owner;
            Options = new ObservableCollection<DialogOption>();
        }

        public void AddOption(DialogOption option)
        {
            Options.Add(option);

        }        

        public void EnableOption(DialogOption option)
        {
            if ( option.Enabled || string.IsNullOrWhiteSpace(option.Label) )
            {
                return;
            }

            option.Enabled = true;           
            option.Target = new DialogPage(mOwner);
            var dv = mOwner.createNewVertex(option.Target); 
            option.Target.PageOwner = this;
            option.Target.OptionOwner = option;
            option.Target.Label = option.LabelBinding;
            option.Target.Vertex = dv;
            option.Parent = this;
            option.Command = option.Command;
            mOwner.AddNode(dv);
            mOwner.LinkNodes(this.Vertex, dv);
            mOwner.Redraw();
        }

        public override string ToString()
        {
            return Label;
        }

        public void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
