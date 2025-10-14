using DA_Assets.DAI;
using DA_Assets.FCU.Model;
using System;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    [Serializable]
    public class SettingsLinker : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] MainSettings mainSettings;
        [SerializeProperty(nameof(mainSettings))]
        public MainSettings MainSettings => monoBeh.Link(ref mainSettings);

        [SerializeField] UITK_Settings uitkSettings;
        [SerializeProperty(nameof(uitkSettings))]
        public UITK_Settings UITK_Settings => monoBeh.Link(ref uitkSettings);

        [SerializeField] ImageSpritesSettings imageSpritesSettings;
        [SerializeProperty(nameof(imageSpritesSettings))]
        public ImageSpritesSettings ImageSpritesSettings => monoBeh.Link(ref imageSpritesSettings);

        [SerializeField] TextureImporterSettings textureImporterSettings;
        public TextureImporterSettings TextureImporterSettings => monoBeh.Link(ref textureImporterSettings);

        [SerializeField] ShadowSettings shadowSettings;
        [SerializeProperty(nameof(shadowSettings))]
        public ShadowSettings ShadowSettings => monoBeh.Link(ref shadowSettings);

        [SerializeField] ButtonSettings buttonSettings;
        [SerializeProperty(nameof(buttonSettings))]
        public ButtonSettings ButtonSettings => monoBeh.Link(ref buttonSettings);

        [SerializeField] LocalizationSettings locSettings;
        [SerializeProperty(nameof(locSettings))]
        public LocalizationSettings LocalizationSettings => monoBeh.Link(ref locSettings);

        [SerializeField] ScriptGeneratorSettings scriptGenSettings;
        [SerializeProperty(nameof(scriptGenSettings))]
        public ScriptGeneratorSettings ScriptGeneratorSettings => monoBeh.Link(ref scriptGenSettings);

        [SerializeField] NovaSettings novaSettings;
        [SerializeProperty(nameof(novaSettings))]
        public NovaSettings NovaSettings => monoBeh.Link(ref novaSettings);

        [SerializeField] JoshPuiSettings joshPuiSettings;
        [SerializeProperty(nameof(joshPuiSettings))]
        public JoshPuiSettings JoshPuiSettings => monoBeh.Link(ref joshPuiSettings);

        [SerializeField] MPUIKitSettings mpuikitSettings;
        [SerializeProperty(nameof(mpuikitSettings))]
        public MPUIKitSettings MPUIKitSettings => monoBeh.Link(ref mpuikitSettings);

        [SerializeField] DttPuiSettings dttPuiSettings;
        [SerializeProperty(nameof(dttPuiSettings))]
        public DttPuiSettings DttPuiSettings => monoBeh.Link(ref dttPuiSettings);

        [SerializeField] SvgImageSettings svgImageSettings;
        [SerializeProperty(nameof(svgImageSettings))]
        public SvgImageSettings SvgImageSettings => monoBeh.Link(ref svgImageSettings);

        [SerializeField] SVGImporterSettings svgImporterSettings;
        [SerializeProperty(nameof(svgImporterSettings))]
        public SVGImporterSettings SVGImporterSettings => monoBeh.Link(ref svgImporterSettings);

        [SerializeField] UnityImageSettings unityImageSettings;
        [SerializeProperty(nameof(unityImageSettings))]
        public UnityImageSettings UnityImageSettings => monoBeh.Link(ref unityImageSettings);

        [SerializeField] RawImageSettings rawImageSettings;
        [SerializeProperty(nameof(rawImageSettings))]
        public RawImageSettings RawImageSettings => monoBeh.Link(ref rawImageSettings);

        [SerializeField] Shapes2DSettings shapes2D_Settings;
        [SerializeProperty(nameof(shapes2D_Settings))]
        public Shapes2DSettings Shapes2DSettings => monoBeh.Link(ref shapes2D_Settings);

        [SerializeField] SpriteRendererSettings srSettings;
        [SerializeProperty(nameof(srSettings))]
        public SpriteRendererSettings SpriteRendererSettings => monoBeh.Link(ref srSettings);

        [SerializeField] TextMeshSettings textMeshSettings;
        [SerializeProperty(nameof(textMeshSettings))]
        public TextMeshSettings TextMeshSettings => monoBeh.Link(ref textMeshSettings);

        [SerializeField] TextFontsSettings textFontsSettings;
        [SerializeProperty(nameof(textFontsSettings))]
        public TextFontsSettings TextFontsSettings => monoBeh.Link(ref textFontsSettings);

        [SerializeField] UnityTextSettings unityTextSettings;
        [SerializeProperty(nameof(unityTextSettings))]
        public UnityTextSettings UnityTextSettings => monoBeh.Link(ref unityTextSettings);

        [SerializeField] PrefabSettings prefabSettings;
        [SerializeProperty(nameof(prefabSettings))]
        public PrefabSettings PrefabSettings => monoBeh.Link(ref prefabSettings);
    }
}