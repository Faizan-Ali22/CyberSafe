//
//███████╗██╗░██████╗░███╗░░░███╗░█████╗░  ░█████╗░░█████╗░███╗░░██╗██╗░░░██╗███████╗██████╗░████████╗███████╗██████╗░
//██╔════╝██║██╔════╝░████╗░████║██╔══██╗  ██╔══██╗██╔══██╗████╗░██║██║░░░██║██╔════╝██╔══██╗╚══██╔══╝██╔════╝██╔══██╗
//█████╗░░██║██║░░██╗░██╔████╔██║███████║  ██║░░╚═╝██║░░██║██╔██╗██║╚██╗░██╔╝█████╗░░██████╔╝░░░██║░░░█████╗░░██████╔╝
//██╔══╝░░██║██║░░╚██╗██║╚██╔╝██║██╔══██║  ██║░░██╗██║░░██║██║╚████║░╚████╔╝░██╔══╝░░██╔══██╗░░░██║░░░██╔══╝░░██╔══██╗
//██║░░░░░██║╚██████╔╝██║░╚═╝░██║██║░░██║  ╚█████╔╝╚█████╔╝██║░╚███║░░╚██╔╝░░███████╗██║░░██║░░░██║░░░███████╗██║░░██║
//╚═╝░░░░░╚═╝░╚═════╝░╚═╝░░░░░╚═╝╚═╝░░╚═╝  ░╚════╝░░╚════╝░╚═╝░░╚══╝░░░╚═╝░░░╚══════╝╚═╝░░╚═╝░░░╚═╝░░░╚══════╝╚═╝░░╚═╝
//
//███████╗░█████╗░██████╗░  ██╗░░░██╗███╗░░██╗██╗████████╗██╗░░░██╗
//██╔════╝██╔══██╗██╔══██╗  ██║░░░██║████╗░██║██║╚══██╔══╝╚██╗░██╔╝
//█████╗░░██║░░██║██████╔╝  ██║░░░██║██╔██╗██║██║░░░██║░░░░╚████╔╝░
//██╔══╝░░██║░░██║██╔══██╗  ██║░░░██║██║╚████║██║░░░██║░░░░░╚██╔╝░░
//██║░░░░░╚█████╔╝██║░░██║  ╚██████╔╝██║░╚███║██║░░░██║░░░░░░██║░░░
//╚═╝░░░░░░╚════╝░╚═╝░░╚═╝  ░╚═════╝░╚═╝░░╚══╝╚═╝░░░╚═╝░░░░░░╚═╝░░░
//

using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Drawers;
using System;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    [Serializable]
    [DisallowMultipleComponent]
    public sealed class FigmaConverterUnity : MonoBehaviour
    {
        private void Start() { }

        public void Reset()
        {
#if DABUTTON_EXISTS
            this.Settings.ButtonSettings.DAB_Settings.Reset();
#endif
        }

        [SerializeField] ProjectImporter importController;
        [SerializeProperty(nameof(importController))]
        public ProjectImporter ProjectImporter => this.Link(ref importController);

        [SerializeField] CanvasDrawer canvasDrawer;
        [SerializeProperty(nameof(canvasDrawer))]
        public CanvasDrawer CanvasDrawer => this.Link(ref canvasDrawer);

#if NOVA_UI_EXISTS
        [SerializeField] NovaDrawer novaDrawer;
        [SerializeProperty(nameof(novaDrawer))]
        public NovaDrawer NovaDrawer => this.Link(ref novaDrawer);
#endif

#if FCU_EXISTS && FCU_UITK_EXT_EXISTS
        [SerializeField] UITK_Converter uitkConverter;
        [SerializeProperty(nameof(uitkConverter))]
        public UITK_Converter UITK_Converter => this.Link(ref uitkConverter);
#endif
        [SerializeField] ProjectCacher projectCacher;
        [SerializeProperty(nameof(projectCacher))]
        public ProjectCacher ProjectCacher => this.Link(ref projectCacher);

        [SerializeField] HashCacher hashCacher;
        [SerializeProperty(nameof(hashCacher))]
        public HashCacher HashCacher => this.Link(ref hashCacher);

        [SerializeField] ProjectDownloader projectDownloader;
        [SerializeProperty(nameof(projectDownloader))]
        public ProjectDownloader ProjectDownloader => this.Link(ref projectDownloader);

        [SerializeField] ImageTypeSetter imageTypeSetter;
        [SerializeProperty(nameof(imageTypeSetter))]
        public ImageTypeSetter ImageTypeSetter => this.Link(ref imageTypeSetter);

        [SerializeField] DuplicateFinder duplicateFinder;
        [SerializeProperty(nameof(duplicateFinder))]
        public DuplicateFinder DuplicateFinder => this.Link(ref duplicateFinder);

        [SerializeField] ScriptGenerator scriptGenerator;
        [SerializeProperty(nameof(scriptGenerator))]
        public ScriptGenerator ScriptGenerator => this.Link(ref scriptGenerator);

        [SerializeField] DelegateHolder delegateHolder;
        [SerializeProperty(nameof(delegateHolder))]
        public DelegateHolder DelegateHolder => this.Link(ref delegateHolder);

        [SerializeField] SettingsLinker settings;
        [SerializeProperty(nameof(settings))]
        public SettingsLinker Settings => this.Link(ref settings);

        [SerializeField] FcuEventHandlers eventHandlers;
        [SerializeProperty(nameof(eventHandlers))]
        public FcuEventHandlers EventHandlers => this.Link(ref eventHandlers);

        [SerializeField] FcuEvents events;
        [SerializeProperty(nameof(events))]
        public FcuEvents Events => this.Link(ref events);

        [SerializeField] CancellationTokenController ctsController;
        [SerializeProperty(nameof(ctsController))]
        public CancellationTokenController CancellationTokenController => this.Link(ref ctsController);

        [SerializeField] PrefabCreator prefabCreator;
        [SerializeProperty(nameof(prefabCreator))]
        public PrefabCreator PrefabCreator => this.Link(ref prefabCreator);

        [SerializeField] PrefabUpdater prefabUpdater;
        [SerializeProperty(nameof(prefabUpdater))]
        public PrefabUpdater PrefabUpdater => this.Link(ref prefabUpdater);

        [SerializeField] FolderCreator folderCreator;
        [SerializeProperty(nameof(folderCreator))]
        public FolderCreator FolderCreator => this.Link(ref folderCreator);

        [SerializeField] InspectorDrawer inspectorDrawer;
        [SerializeProperty(nameof(inspectorDrawer))]
        public InspectorDrawer InspectorDrawer => this.Link(ref inspectorDrawer);

        [SerializeField] Authorizer authorizer;
        [SerializeProperty(nameof(authorizer))]
        public Authorizer Authorizer => this.Link(ref authorizer);

        [SerializeField] RequestSender requestSender;
        [SerializeProperty(nameof(requestSender))]
        public RequestSender RequestSender => this.Link(ref requestSender);

        [SerializeField] HashGenerator hashGenerator;
        [SerializeProperty(nameof(hashGenerator))]
        public HashGenerator HashGenerator => this.Link(ref hashGenerator);

        [SerializeField] NameHumanizer nameHumanizer;
        [SerializeProperty(nameof(nameHumanizer))]
        public NameHumanizer NameHumanizer => this.Link(ref nameHumanizer);

        [SerializeField] FontDownloader fontDownloader;
        [SerializeProperty(nameof(fontDownloader))]
        public FontDownloader FontDownloader => this.Link(ref fontDownloader);

        [SerializeField] FontLoader fontLoader;
        [SerializeProperty(nameof(fontLoader))]
        public FontLoader FontLoader => this.Link(ref fontLoader);

        [SerializeField] GraphicHelpers graphicHelpers;
        [SerializeProperty(nameof(graphicHelpers))]
        public GraphicHelpers GraphicHelpers => this.Link(ref graphicHelpers);

        [SerializeField] TagSetter tagSetter;
        [SerializeProperty(nameof(tagSetter))]
        public TagSetter TagSetter => this.Link(ref tagSetter);

        [SerializeField] SpriteProcessor spriteProcessor;
        [SerializeProperty(nameof(spriteProcessor))]
        public SpriteProcessor SpriteProcessor => this.Link(ref spriteProcessor);

        [SerializeField] AssetTools tools;
        [SerializeProperty(nameof(tools))]
        public AssetTools AssetTools => this.Link(ref tools);

        [SerializeField] NameSetter nameSetter;
        [SerializeProperty(nameof(nameSetter))]
        public NameSetter NameSetter => this.Link(ref nameSetter);

        [SerializeField] SyncHelpers syncHelper;
        [SerializeProperty(nameof(syncHelper))]
        public SyncHelpers SyncHelpers => this.Link(ref syncHelper);

        [SerializeField] TransformSetter transformSetter;
        [SerializeProperty(nameof(transformSetter))]
        public TransformSetter TransformSetter => this.Link(ref transformSetter);

        [SerializeField] CurrentProject currentProject;
        [SerializeProperty(nameof(currentProject))]
        public CurrentProject CurrentProject => this.Link(ref currentProject);

        [SerializeField] SpriteGenerator spriteGenerator;
        [SerializeProperty(nameof(spriteGenerator))]
        public SpriteGenerator SpriteGenerator => this.Link(ref spriteGenerator);

        [SerializeField] SpriteColorizer spriteColorizer;
        [SerializeProperty(nameof(spriteColorizer))]
        public SpriteColorizer SpriteColorizer => this.Link(ref spriteColorizer);

        [SerializeField] SpritePathSetter spritePathSetter;
        [SerializeProperty(nameof(spritePathSetter))]
        public SpritePathSetter SpritePathSetter => this.Link(ref spritePathSetter);

        [SerializeField] SpriteDownloader spriteDownloader;
        [SerializeProperty(nameof(spriteDownloader))]
        public SpriteDownloader SpriteDownloader => this.Link(ref spriteDownloader);

        [SerializeField] SpriteSlicer spriteSlicer;
        [SerializeProperty(nameof(spriteSlicer))]
        public SpriteSlicer SpriteSlicer => this.Link(ref spriteSlicer);

        [SerializeField] PreImportDataCreator preImportDataCreator;
        [SerializeProperty(nameof(preImportDataCreator))]
        public PreImportDataCreator PreImportDataCreator => this.Link(ref preImportDataCreator);

        [SerializeField] string guid;
        public string Guid => guid.CreateShortGuid(out guid);
    }
}