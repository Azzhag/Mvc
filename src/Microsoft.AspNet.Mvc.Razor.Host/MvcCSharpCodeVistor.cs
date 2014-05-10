// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
// WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF
// TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR
// NON-INFRINGEMENT.
// See the Apache 2 License for the specific language governing
// permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Razor.Generator;
using Microsoft.AspNet.Razor.Generator.Compiler;
using Microsoft.AspNet.Razor.Generator.Compiler.CSharp;
using Microsoft.AspNet.Razor.Parser.SyntaxTree;

namespace Microsoft.AspNet.Mvc.Razor
{
    public class MvcCSharpCodeVisitor : CSharpCodeVisitor
    {
        private readonly List<InjectChunk> _injectChunks = new List<InjectChunk>();

        public MvcCSharpCodeVisitor(CSharpCodeWriter writer, CodeGeneratorContext context) 
            : base(writer, context)
        {
            
        }

        public override void Accept(Chunk chunk)
        {
            var injectChunk = chunk as InjectChunk;
            if (injectChunk != null)
            {
                Visit(injectChunk);
            }
            base.Accept(chunk);
        }

        protected override void Visit(InjectChunk chunk)
        {
            _injectChunks.Add(chunk);
        }
    }
}