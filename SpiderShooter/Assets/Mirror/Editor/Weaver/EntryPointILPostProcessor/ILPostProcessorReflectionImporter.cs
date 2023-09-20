// based on paul's resolver from
// https://github.com/MirageNet/Mirage/commit/def64cd1db525398738f057b3d1eb1fe8afc540c?branch=def64cd1db525398738f057b3d1eb1fe8afc540c&diff=split
//
// ILPostProcessorAssemblyRESOLVER does not find the .dll file for:
// "System.Private.CoreLib"
// we need this custom reflection importer to fix that.
using Mono.CecilX;
using System.Linq;
using System.Reflection;

namespace Mirror.Weaver
{
    internal class ILPostProcessorReflectionImporter : DefaultReflectionImporter
    {
        private const string SystemPrivateCoreLib = "System.Private.CoreLib";
        private readonly AssemblyNameReference fixedCoreLib;

        public ILPostProcessorReflectionImporter(ModuleDefinition module)
            : base(module)
        {
            // find the correct library for "System.Private.CoreLib".
            // either mscorlib or netstandard.
            // defaults to System.Private.CoreLib if not found.
            fixedCoreLib = module.AssemblyReferences.FirstOrDefault(
                a => a.Name is "mscorlib" or "netstandard" or SystemPrivateCoreLib
            );
        }

        public override AssemblyNameReference ImportReference(AssemblyName name)
        {
            // System.Private.CoreLib?
            if (name.Name == SystemPrivateCoreLib && fixedCoreLib != null)
            {
                return fixedCoreLib;
            }

            // otherwise import as usual
            return base.ImportReference(name);
        }
    }
}
