using RSMacroProgramApi.MacroApi.RS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RSMacroProgramApi.MacroApi.Generic
{
    //TODO: Add documentation to all the members in IInteractionObject;
    public interface IInteractionObject
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
        Size AdBanner { get; } //TODO: remove this, since it will be available at osrsconstants, and rs3constants
        RSGame game { get; }
        Rectangle TargetWindow { get; }
        bool HasAdBanner { get; }
        
    }

    public interface IAbstractScript
    {
        //IInteractionObject api { get; set; }

        IInteractionObject getApi();
        void _setApi(IInteractionObject newApi);

        void init();
        void start();
        void tick();
        void dispose();
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
