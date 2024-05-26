using Godot;

public partial class Canvas3D : Sprite3D
{
    [Export]
    private Viewport viewport;

    public override void _Ready()
    {
        Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
        Texture = viewport.GetTexture();
        viewport.TransparentBg = true;
    }
}