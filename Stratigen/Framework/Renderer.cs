using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
//using SharpDX.Direct3D.Shaders;

namespace Stratigen.Framework
{
    public class Renderer : Window
    {
        private SwapChain _swapChain;
        private Device _device;
        private RenderTargetView _backBufferView;
        private DepthStencilView _zBufferView;
        private DeviceContext _context;
        private int _refresh = 60;
        private IntPtr _handle;
        //private 

        private RasterizerState _rasterState;
        private Texture2D _backBufferTexture;

        public Device Device
        {
            get
            {
                return _device;
            }
        }

        public SwapChain SwapChain
        {
            get
            {
                return _swapChain;
            }
        }

        public DeviceContext Context
        {
            get
            {
                return _context;
            }
        }

        public RenderTargetView BackBufferView
        {
            get
            {
                return _backBufferView;
            }
        }

        public DepthStencilView ZBufferView
        {
            get
            {
                return _zBufferView;
            }
        }

        public new int Width
        {
            get
            {
                return (int)base.Width;
            }
        }

        public new int Height
        {
            get
            {
                return (int)base.Height;
            }
        }

        public int Refresh
        {
            get
            {
                return _refresh;
            }
            set
            {
                _refresh = value;
            }
        }

        public IntPtr Handle
        {
            get
            {
                return _handle;
            }
        }

        public Renderer(int width, int height, bool windowed = true)
        {
            //Window Handle
            _handle = new WindowInteropHelper(this).Handle;
            //Base Assignments
            base.Width = width;
            base.Height = height;
            base.ResizeMode = System.Windows.ResizeMode.CanResize;
            //Event Handlers
            base.PreviewKeyDown += Renderer_PreviewKeyDown;
            base.SizeChanged += Renderer_SizeChanged;
            //Swap Chain Description
            SwapChainDescription scd = new SwapChainDescription
            {
                BufferCount = 1,
                IsWindowed = windowed,
                SwapEffect = SwapEffect.Discard,
                OutputHandle = Handle,
                Usage = Usage.RenderTargetOutput,
                ModeDescription = new ModeDescription(Width, Height, new Rational(Refresh, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(4, 4),
            };
            //Feature Levels
            FeatureLevel[] FeatureLevels = new FeatureLevel[] { FeatureLevel.Level_11_0, FeatureLevel.Level_10_1, FeatureLevel.Level_10_0 };
            //Device & SwapChain Creation
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, scd, out _device, out _swapChain);
            //Context
            _context = Device.ImmediateContext;
            //Not real sure what this is for - I get that it disables response to Alt+Enter, but why? Who cares? Is it worth the processing time even?
            using (var factory = SwapChain.GetParent<Factory>())
            {
                factory.MakeWindowAssociation(Handle, WindowAssociationFlags.IgnoreAltEnter);
            }
        }

        void Renderer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _backBufferTexture.Dispose();
            _backBufferView.Dispose();
            _zBufferView.Dispose();
            SwapChain.ResizeBuffers(1, Width, Height, Format.B8G8R8A8_UNorm, SwapChainFlags.AllowModeSwitch);
            _backBufferTexture = _swapChain.GetBackBuffer<Texture2D>(0);
            _backBufferView = new RenderTargetView(_device, _backBufferTexture);
            using (Texture2D zBufferTexture = new Texture2D(_device, 
                new Texture2DDescription
                {
                    Format = Format.D16_UNorm,
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = Width,
                    Height = Height,
                    SampleDescription = new SampleDescription(4, 4),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                }
            )) _zBufferView = new DepthStencilView(_device, zBufferTexture);
            Context.Rasterizer.SetViewport(0, 0, Width, Height);
            Context.OutputMerger.SetTargets(_zBufferView, _backBufferView);

            if (RendererResizeHandler != null) RendererResizeHandler(e);
        }

        void Renderer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (RendererKeyboardHandler != null) RendererKeyboardHandler(e);
        }

        public delegate void KeyboardHandler(KeyEventArgs e);
        public KeyboardHandler RendererKeyboardHandler;

        public delegate void ResizeHandler(SizeChangedEventArgs e);
        public ResizeHandler RendererResizeHandler;

        public void SetDrawMode(PrimitiveTopology topology)
        {
            _context.InputAssembler.PrimitiveTopology = topology;
        }

        public void Clear(Color4 color)
        {
            _context.ClearRenderTargetView(_backBufferView, color);
            _context.ClearDepthStencilView(_zBufferView, DepthStencilClearFlags.Depth, 1f, 0);
        }

        /// <summary>
        /// Render things, uses deprecated "Present" method on the swap chain.
        /// </summary>
        public void Draw()
        {
            _swapChain.Present(0, PresentFlags.None);
        }

        public void ResetRasterState()
        {
            _rasterState.Dispose();
            _rasterState = new RasterizerState(_device, RasterizerStateDescription.Default());
            _context.Rasterizer.State = _rasterState;
        }

        public void WireframeRasterState()
        {
            _rasterState.Dispose();
            RasterizerStateDescription rsd = RasterizerStateDescription.Default();
            rsd.FillMode = FillMode.Wireframe;
            _rasterState = new RasterizerState(_device, rsd);
            _context.Rasterizer.State = _rasterState;
        }
    }
}
