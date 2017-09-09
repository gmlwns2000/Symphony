using MMDFileParser.PMXModelParser;
using MMF.Bone;

namespace MMF.Model.PMX
{
    public class PMXModelWithPhysics : PMXModel
    {
        public PMXModelWithPhysics(ModelData modeldata, ISubresourceLoader subResourceLoader, string filename) : base(modeldata, subResourceLoader, filename)
        {
        }

        public new static PMXModelWithPhysics OpenLoad(string filePath, RenderContext context)
        {
            PMXModelWithPhysics pMXModelWithPhysics = (PMXModelWithPhysics)PMXModelWithPhysics.FromFile(filePath);
            pMXModelWithPhysics.Load(context);
            return pMXModelWithPhysics;
        }

        public new static PMXModel FromFile(string filePath)
        {
            string directoryName = System.IO.Path.GetDirectoryName(filePath);
            return PMXModelWithPhysics.FromFile(filePath, directoryName);
        }

        public new static PMXModel FromFile(string filePath, string textureFolder)
        {
            return PMXModelWithPhysics.FromFile(filePath, new BasicSubresourceLoader(textureFolder));
        }

        public new static PMXModel FromFile(string filePath, ISubresourceLoader loader)
        {
            PMXModel result;
            using (System.IO.FileStream fileStream = System.IO.File.OpenRead(filePath))
            {
                result = new PMXModelWithPhysics(ModelData.GetModel(fileStream), loader, System.IO.Path.GetFileName(filePath));
            }
            return result;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override ISkinningProvider InitializeSkinning()
        {
            return new PMXSkeletonWithPhysics(Model);
        }
    }
}
