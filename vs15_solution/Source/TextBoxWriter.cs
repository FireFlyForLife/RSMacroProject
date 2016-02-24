using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RSMacroProgram
{
    class TextBoxWriter : TextWriter
    {
        TextBox _output = null;

        public TextBoxWriter(TextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            _output.AppendText(value.ToString()); 
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }

    class MultiTextWriter : TextWriter
    {
        private List<TextWriter> writers = new List<TextWriter>();

        public MultiTextWriter(params TextWriter[] writer)
        {
            foreach(TextWriter w in writers) writers.Add(w);
        }

        public override void Write(char value)
        {
            base.Write(value);
            foreach (TextWriter w in writers) w.Write(value);
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
