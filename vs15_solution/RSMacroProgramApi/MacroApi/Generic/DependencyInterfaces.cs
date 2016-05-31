using RSMacroProgramApi.MacroApi.RS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RSMacroProgramApi.MacroApi.Generic
{
    public interface IInterractionObject
    {
        //I/O
        Point MousePosition();
        bool Move(int x, int y);
        bool Click(int key);
        bool MousePress(int key);
        bool MouseRelease(int key);
        bool InputKey(char c);
        bool InputKeys(String str);
        bool PressKey(char c);
        bool ReleaseKey(char c);

        //Data access
        Size AdBanner { get; }
        RSGame game { get; }
        Rectangle TargetWindow { get; }
        bool HasAdBanner { get; }
        
    }

    public interface Paintable
    {
        void paint(Graphics g);
    }

    public interface Clickable
    {
        void Click();
    }

    public interface Pointable
    {
        Point nextPoint();
    }

    public interface Openable
    {
        void Open();
    }

    public interface Closeable
    {
        void Close();
    }
}
