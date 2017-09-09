using System.Collections.Generic;
using SlimDX.Direct3D10;

namespace DirectCanvas.Rendering.Sprites
{
    static class SpriteRendererBlendStateHelper
    {
        private static void SetDefaults(ref BlendStateDescription blendDesc)
        {
            for (uint i = 0; i < 8; i++)
            {
                blendDesc.SetWriteMask(i, ColorWriteMaskFlags.All);
                blendDesc.SetBlendEnable(i, true);
            }
        }

        internal static Dictionary<BlendStateMode, BlendState> InitializeDefaultBlendStates(Device device)
        {
            var blendStates = new Dictionary<BlendStateMode, BlendState>();

            ///////////////////////////////////////////////////////////////////////////
            //// AlphaBlend //////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////
            var blendDesc = new BlendStateDescription();

            blendDesc.IsAlphaToCoverageEnabled = false;
            blendDesc.BlendOperation = BlendOperation.Add;
            blendDesc.AlphaBlendOperation = BlendOperation.Add;

            blendDesc.SourceAlphaBlend = BlendOption.One;
            blendDesc.DestinationAlphaBlend = BlendOption.One;

            blendDesc.SourceBlend = BlendOption.One;
            blendDesc.DestinationBlend = BlendOption.InverseSourceAlpha;
            
            SetDefaults(ref blendDesc);

            var blendstate = BlendState.FromDescription(device, blendDesc);
            blendStates.Add(BlendStateMode.AlphaBlend, blendstate);

            ///////////////////////////////////////////////////////////////////////////
            //// Subtractive /////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////
            blendDesc = new BlendStateDescription();
            blendDesc.IsAlphaToCoverageEnabled = false;

            blendDesc.BlendOperation = BlendOperation.ReverseSubtract;
            blendDesc.AlphaBlendOperation = BlendOperation.ReverseSubtract;

            blendDesc.SourceAlphaBlend = BlendOption.SourceAlpha;
            blendDesc.DestinationAlphaBlend = BlendOption.One;

            blendDesc.SourceBlend = BlendOption.SourceAlpha;
            blendDesc.DestinationBlend = BlendOption.One;

            SetDefaults(ref blendDesc);

            blendstate = BlendState.FromDescription(device, blendDesc);
            blendStates.Add(BlendStateMode.Subtractive, blendstate);

            ///////////////////////////////////////////////////////////////////////////
            //// Additive /////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////
            blendDesc = new BlendStateDescription();
            blendDesc.IsAlphaToCoverageEnabled = false;

            blendDesc.BlendOperation = BlendOperation.Add;
            blendDesc.AlphaBlendOperation = BlendOperation.Add;

            blendDesc.SourceAlphaBlend = BlendOption.Zero;
            blendDesc.DestinationAlphaBlend = BlendOption.Zero;

            blendDesc.SourceBlend = BlendOption.SourceColor;
            blendDesc.DestinationBlend = BlendOption.One;

            SetDefaults(ref blendDesc);

            blendstate = BlendState.FromDescription(device, blendDesc);
            blendStates.Add(BlendStateMode.Additive, blendstate);


            ///////////////////////////////////////////////////////////////////////////
            //// Copy //////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            blendDesc = new BlendStateDescription();
            blendDesc.IsAlphaToCoverageEnabled = false;

            blendDesc.BlendOperation = BlendOperation.Add;
            blendDesc.AlphaBlendOperation = BlendOperation.Add;

            blendDesc.SourceAlphaBlend = BlendOption.SourceAlpha;
            blendDesc.DestinationAlphaBlend = BlendOption.SourceAlpha;

            blendDesc.SourceBlend = BlendOption.SourceAlpha;
            blendDesc.DestinationBlend = BlendOption.Zero;

            SetDefaults(ref blendDesc);

            blendstate = BlendState.FromDescription(device, blendDesc);
            blendStates.Add(BlendStateMode.Copy, blendstate);


            ///////////////////////////////////////////////////////////////////////////
            //// SourceOver //////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            blendDesc = new BlendStateDescription();
            blendDesc.IsAlphaToCoverageEnabled = false;

            blendDesc.BlendOperation = BlendOperation.Add;
            blendDesc.AlphaBlendOperation = BlendOperation.Add;

            blendDesc.SourceAlphaBlend = BlendOption.One;
            blendDesc.DestinationAlphaBlend = BlendOption.One;

            blendDesc.SourceBlend = BlendOption.One;
            blendDesc.DestinationBlend = BlendOption.InverseSourceAlpha;

            SetDefaults(ref blendDesc);

            blendstate = BlendState.FromDescription(device, blendDesc);
            blendStates.Add(BlendStateMode.SourceOver, blendstate);

            ///////////////////////////////////////////////////////////////////////////
            //// SourceATop //////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            blendDesc = new BlendStateDescription();
            blendDesc.IsAlphaToCoverageEnabled = false;

            blendDesc.BlendOperation = BlendOperation.Add;
            blendDesc.AlphaBlendOperation = BlendOperation.Add;

            blendDesc.SourceAlphaBlend = BlendOption.DestinationAlpha;
            blendDesc.DestinationAlphaBlend = BlendOption.DestinationAlpha;

            blendDesc.SourceBlend = BlendOption.DestinationAlpha;
            blendDesc.DestinationBlend = BlendOption.InverseSourceAlpha;

            SetDefaults(ref blendDesc);

            blendstate = BlendState.FromDescription(device, blendDesc);
            blendStates.Add(BlendStateMode.SourceATop, blendstate);


            ///////////////////////////////////////////////////////////////////////////
            //// SourceIn //////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            blendDesc = new BlendStateDescription();
            blendDesc.IsAlphaToCoverageEnabled = false;

            blendDesc.BlendOperation = BlendOperation.Add;
            blendDesc.AlphaBlendOperation = BlendOperation.Add;

            blendDesc.SourceAlphaBlend = BlendOption.DestinationAlpha;
            blendDesc.DestinationAlphaBlend = BlendOption.Zero;

            blendDesc.SourceBlend = BlendOption.DestinationAlpha;
            blendDesc.DestinationBlend = BlendOption.Zero;

            SetDefaults(ref blendDesc);

            blendstate = BlendState.FromDescription(device, blendDesc);
            blendStates.Add(BlendStateMode.SourceIn, blendstate);

            ///////////////////////////////////////////////////////////////////////////
            //// SourceOut //////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            blendDesc = new BlendStateDescription();
            blendDesc.IsAlphaToCoverageEnabled = false;

            blendDesc.BlendOperation = BlendOperation.Add;
            blendDesc.AlphaBlendOperation = BlendOperation.Add;

            blendDesc.SourceAlphaBlend = BlendOption.InverseDestinationAlpha;
            blendDesc.DestinationAlphaBlend = BlendOption.Zero;

            blendDesc.SourceBlend = BlendOption.SourceColor;
            blendDesc.DestinationBlend = BlendOption.Zero;

            SetDefaults(ref blendDesc);

            blendstate = BlendState.FromDescription(device, blendDesc);
            blendStates.Add(BlendStateMode.SourceOut, blendstate);


            ///////////////////////////////////////////////////////////////////////////
            //// DestinationIn //////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            blendDesc = new BlendStateDescription();
            blendDesc.IsAlphaToCoverageEnabled = false;

            blendDesc.BlendOperation = BlendOperation.Add;
            blendDesc.AlphaBlendOperation = BlendOperation.Add;

            blendDesc.SourceAlphaBlend = BlendOption.DestinationAlpha;
            blendDesc.DestinationAlphaBlend = BlendOption.Zero;

            blendDesc.SourceBlend = BlendOption.Zero;
            blendDesc.DestinationBlend = BlendOption.SourceAlpha;

            SetDefaults(ref blendDesc);

            blendstate = BlendState.FromDescription(device, blendDesc);
            blendStates.Add(BlendStateMode.DestinationIn, blendstate);


            ///////////////////////////////////////////////////////////////////////////
            //// DestinationOver //////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            blendDesc = new BlendStateDescription();
            blendDesc.IsAlphaToCoverageEnabled = false;

            blendDesc.BlendOperation = BlendOperation.Add;
            blendDesc.AlphaBlendOperation = BlendOperation.Add;

            blendDesc.SourceAlphaBlend = BlendOption.One;
            blendDesc.DestinationAlphaBlend = BlendOption.InverseDestinationAlpha;

            blendDesc.SourceBlend = BlendOption.One;
            blendDesc.DestinationBlend = BlendOption.One;

            SetDefaults(ref blendDesc);

            blendstate = BlendState.FromDescription(device, blendDesc);
            blendStates.Add(BlendStateMode.DestinationOver, blendstate);


            ///////////////////////////////////////////////////////////////////////////
            //// DestinationOut //////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            blendDesc = new BlendStateDescription();
            blendDesc.IsAlphaToCoverageEnabled = false;

            blendDesc.BlendOperation = BlendOperation.Add;
            blendDesc.AlphaBlendOperation = BlendOperation.Add;

            blendDesc.SourceAlphaBlend = BlendOption.InverseSourceAlpha;
            blendDesc.DestinationAlphaBlend = BlendOption.InverseSourceAlpha;

            blendDesc.SourceBlend = BlendOption.Zero;
            blendDesc.DestinationBlend = BlendOption.One;

            SetDefaults(ref blendDesc);

            blendstate = BlendState.FromDescription(device, blendDesc);
            blendStates.Add(BlendStateMode.DestinationOut, blendstate);

            ///////////////////////////////////////////////////////////////////////////
            //// DestinationATop //////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////
            blendDesc = new BlendStateDescription();
            blendDesc.IsAlphaToCoverageEnabled = false;

            blendDesc.BlendOperation = BlendOperation.Add;
            blendDesc.AlphaBlendOperation = BlendOperation.Add;

            blendDesc.SourceAlphaBlend = BlendOption.InverseDestinationAlpha;
            blendDesc.DestinationAlphaBlend = BlendOption.SourceAlpha;

            blendDesc.SourceBlend = BlendOption.One;
            blendDesc.DestinationBlend = BlendOption.One;

            SetDefaults(ref blendDesc);

            blendstate = BlendState.FromDescription(device, blendDesc);
            blendStates.Add(BlendStateMode.DestinationATop, blendstate);


            return blendStates;
        }
    }
}