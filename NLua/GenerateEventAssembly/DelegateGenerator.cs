/*
 * This file is part of NLua.
 * 
 * Copyright (c) 2015 Vinicius Jarina (viniciusjarina@gmail.com)
 * Copyright (C) 2003-2005 Fabio Mascarenhas de Queiroz.
 * Copyright (C) 2012 Megax <http://megax.yeahunter.hu/>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;

namespace NLua
{
	using LuaCore  = KeraLua.Lua;
	using LuaState = KeraLua.LuaState;

	/*
	 * Class used for generating delegates that get a function from the Lua
	 * stack as a delegate of a specific type.
	 * 
	 * Author: Fabio Mascarenhas
	 * Version: 1.0
	 */
	class DelegateGenerator
	{
		private ObjectTranslator translator;
		private Type delegateType;
		

		public DelegateGenerator (ObjectTranslator objectTranslator, Type type)
		{
			translator = objectTranslator;
			delegateType = type;
		}

		public object ExtractGenerated (LuaState luaState, int stackPos)
		{
			return CodeGeneration.Instance.GetDelegate (delegateType, translator.GetFunction (luaState, stackPos));
		}
	}
}