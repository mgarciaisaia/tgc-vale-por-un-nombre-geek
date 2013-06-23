using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using System.Reflection;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    class Modifiers
    {
        #region Miembros
        private static Modifiers instance;
        private List<Binding> modifierBindings;

        private struct Binding
        {
            public object obj;
            public PropertyInfo property;
            public string varName;

            public Binding(string varName, object obj, string propertyName)
            {
                this.property = obj.GetType().GetProperty(propertyName);
                this.varName = varName;
                this.obj = obj;
            }

            public Binding(string varName, Type classType, string staticPropertyName)
            {
                this.property = classType.GetProperty(staticPropertyName);
                this.varName = varName;
                this.obj = null;
            }
        }
        #endregion


        #region Initizalize
        private Modifiers()
        {
            modifierBindings = new List<Binding>();      
        }

        public static void initialize()
        {
            instance = new Modifiers();
        }

        public static Modifiers Instance
        {
            get
            {
                if (instance == null) initialize();
                return instance;
            }
        }

        #endregion


        #region Getters

        public T get<T>(string varName){
            return (T)GuiController.Instance.Modifiers[varName];
        }

        public void get(string varName, out object obj)
        {
            obj = GuiController.Instance.Modifiers[varName];
        }

        #endregion


        #region Bindings
        /// <summary>
        /// Enlaza el valor del modifier con el de la propiedad del objeto. Es unidireccional (modifier=>propiedad).
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        public void bind(string varName, object obj, string propertyName)
        {
            this.modifierBindings.Add(new Binding(varName, obj, propertyName));
        }

        /// <summary>
        /// Enlaza el valor del modifier con el de la propiedad de la clase. Es unidireccional (modifier=>propiedad).
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="classType"></param>
        /// <param name="staticPropertyName"></param>
        public void bind(string varName, Type classType, string staticPropertyName)
        {
            this.modifierBindings.Add(new Binding(varName, classType, staticPropertyName));
        }


        /// <summary>
        /// Si el valor de un modifier difiere del de la propiedad asociada, setea el valor.
        /// </summary>
        public void update()
        {

            foreach (Binding b in modifierBindings)
            {
                object value = GuiController.Instance.Modifiers.getValue(b.varName);
                if (!value.Equals(b.property.GetValue(b.obj, null))) b.property.SetValue(b.obj, value, null);

            }

        }
        #endregion





        public static void clear()
        {
            if (instance != null) instance=null;
        }
    }
}
