using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using KeraLua;


namespace LuaSample
{
	public partial class LuaSampleViewController : UIViewController
	{
		IntPtr state;
		public static Lua.lua_CFunction g_print = print;

		public void lua_setglobal(string name)
		{
			Lua.lua_pushstring (state,name);
			Lua.lua_insert (state,-2);
			Lua.lua_settable (state, (int)-10002);
		}

		public LuaSampleViewController () : base ("LuaSampleViewController", null)
		{
			state = Lua.luaL_newstate ();
			Lua.luaL_openlibs (state);
			Lua.lua_pushcfunction (state, print);
			lua_setglobal ("print");
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		[Action ("OnClickEval:")]
		void OnClickEval (MonoTouch.Foundation.NSObject sender)
		{
			int error = DoString (textView.Text);

			Console.WriteLine (" Executed: return code = " + error);
		}

		[MonoTouch.MonoPInvokeCallback (typeof (Lua.lua_CFunction))]
		static int print (Lua.lua_State L)
		{
			int n = Lua.lua_gettop(L);  /* number of arguments */
			int i;
			uint len;
			for (i=1; i<=n; i++) {
				string s = Lua.lua_tolstring (L, i, out len).ToString (len);
				Console.Write (s);
			}
			Console.WriteLine();
			return 0;
		}

		public int DoString(string chunk)
		{
			int result = Lua.luaL_loadstring(state, chunk);
			if(result != 0)
				return result;
			
			return Lua.lua_pcall(state, 0, -1, 0);
		}
	}
}

