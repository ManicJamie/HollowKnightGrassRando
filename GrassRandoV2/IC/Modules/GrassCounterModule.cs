using GrassCore;
using GrassRando;
using GrassRando.IC;
using ItemChanger.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GrassRandoV2.IC.Modules
{
    /// <summary>
    /// Attaches a happy little grass counter to the geo counter :)
    /// </summary>
    internal class GrassCounterModule : Module
    {
        private GrassCounter? grassCounter;

        private Behaviour? UtilBehaviour;

        public override void Initialize()
        {
            UtilBehaviour ??= Behaviour.CreateBehaviour();

            GrassItem.GrassGiven += UpdateGrassCount;

            LocationRegistrar.Instance.UpdateGrassRoomCount += UpdateLocalGrassCount;

            UtilBehaviour.OnUpdate += EnsureGrassCountAttached;
        }

        private void EnsureGrassCountAttached(object _, EventArgs _1)
        {
            var geoCounter = GameManager.instance?.hero_ctrl?.geoCounter?.gameObject;
            grassCounter = geoCounter?.GetComponent<GrassCounter>();

            if (geoCounter != null && grassCounter == null)
            {
                geoCounter.AddComponent<GrassCounter>();
                grassCounter = geoCounter.GetComponent<GrassCounter>();
                UpdateGrassCount(GrassRandoMod.Instance.saveData.grassCount);
            }
        }

        public override void Unload()
        {
            GrassItem.GrassGiven -= UpdateGrassCount;

            UtilBehaviour!.OnUpdate -= EnsureGrassCountAttached;

            var geoCounter = GameManager.instance?.hero_ctrl?.geoCounter?.gameObject;

            if (geoCounter != null)
            {
                GameObject.Destroy(geoCounter.GetComponent<GrassCounter>());
                grassCounter = null;
            }
        }

        private void UpdateGrassCount(int newCount)
        {
            grassCounter?.UpdateGrassCount(newCount);
        }

        private void UpdateLocalGrassCount((int, int) countTuple)
        {
            var (newCount, totalCount) = countTuple;
            grassCounter?.UpdateLocalGrassCount(newCount, totalCount);
        }
    }

    // Stripped down counter from GrassyKnight.
    class Behaviour : MonoBehaviour
    {
        public event EventHandler? OnUpdate;

        public void Update()
        {
            OnUpdate?.Invoke(this, EventArgs.Empty);
        }

        public static Behaviour CreateBehaviour()
        {
            // A game object that will do nothing quietly in the corner until
            // the end of time.
            GameObject dummy = new("Behavior Container", typeof(Behaviour));
            DontDestroyOnLoad(dummy);

            return dummy.GetComponent<Behaviour>();
        }
    }

    class GrassCounter : MonoBehaviour
    {
        private class RowLayoutObject
        {
            // The object will be considered to be at least this wide when
            // laying out items, even if its actual bounds are smaller.
            public float MinWidth = 0;

            // If the object's layout width needs to be increased, it will
            // increased in steps of this. So if the real width was 10, the min
            // width was 9, and the step size was 3, the computed width would
            // be 12.
            private float _widthStepSize = 1;
            public float WidthStepSize
            {
                get => _widthStepSize;
                set
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(
                            $"WidthStepSize must not be negative, got {value}");
                    }

                    _widthStepSize = value;
                }
            }

            // This will be added to the real width of object as the first step
            // of calculating the computed width.
            public float PaddingRight = 0;

            public GameObject? GameObject_ = null;

            public float GetRealWidth()
            {
                Renderer? renderer = (GameObject_?.GetComponent<Renderer>()) ?? throw new InvalidOperationException("GameObject_ must be non-null and have a renderer.");
                Transform parentTransform = GameObject_.transform.parent;
                if (parentTransform == null)
                {
                    return renderer.bounds.size.x;
                }
                else
                {
                    Vector3 localSize =
                        parentTransform.InverseTransformVector(
                            renderer.bounds.size);
                    return localSize.x;
                }
            }

            public float GetComputedWidth()
            {
                float realWidth = GetRealWidth();
                float paddedRealWidth = realWidth + PaddingRight;
                float unroundedComputedWidth = Mathf.Max(
                    paddedRealWidth, MinWidth);
                if (unroundedComputedWidth <= MinWidth)
                {
                    return MinWidth;
                }
                else if (WidthStepSize <= 0)
                {
                    return unroundedComputedWidth;
                }
                else
                {
                    return MinWidth + WidthStepSize * Mathf.Ceil(
                        (unroundedComputedWidth - MinWidth) / WidthStepSize);
                }
            }
        }

        // The first object is the "anchor", it will not be moved but its
        // computed width will be used.
        private List<RowLayoutObject> _layout = new List<RowLayoutObject>();

        // The normal size of the geo count is rather large, such that adding
        // a bunch of grass stats next to it is overwhelmingly large. So we
        // scale it down by this factor.
        public float Scale = 0.6f;

        private GameObject? _globalCount = null;
        private GameObject? _localCount = null;

        public void Start()
        {
            _Start();
        }

        private void _Start()
        {
            _layout.Add(new RowLayoutObject
            {
                MinWidth = 1.4f, // A bit wider than 3 digits
                WidthStepSize = 0.5f, // Roughly 1 digit
                PaddingRight = 0.2f,
                GameObject_ = GetGeoTextObject(),
            });
            _layout.Add(new RowLayoutObject
            {
                MinWidth = 0,
                WidthStepSize = 0,
                PaddingRight = 0.2f,
                GameObject_ = CreateSpriteObject(
                    "Grass Sprite", "grassIcon.png"),
            });
            _globalCount = CreateTextObject("Global Grass Count");
            _layout.Add(new RowLayoutObject
            {
                MinWidth = 0,
                WidthStepSize = 0.5f,
                PaddingRight = 0.2f,
                GameObject_ = _globalCount,
            });
            _layout.Add(new RowLayoutObject
            {
                MinWidth = 0,
                WidthStepSize = 0,
                PaddingRight = 0.2f,
                GameObject_ = CreateSpriteObject(
                   "Grass Sprite", "grassIcon.png"), //TODO: change sprite or move below existing count
            });
            _localCount = CreateTextObject("Local Grass Count");
            _layout.Add(new RowLayoutObject
            {
                MinWidth = 0,
                WidthStepSize = 0,
                PaddingRight = 0,
                GameObject_ = _localCount,
            });
        }

        private GameObject CreateTextObject(string name)
        {
            GameObject result = new GameObject(
                name, typeof(TextMesh), typeof(MeshRenderer));
            result.layer = gameObject.layer;
            UnityEngine.Object.DontDestroyOnLoad(result);

            GameObject geoTextObject = GetGeoTextObject();

            TextMesh geoTextMesh = geoTextObject.GetComponent<TextMesh>();
            TextMesh textMesh = result.GetComponent<TextMesh>();
            textMesh.alignment = TextAlignment.Left;
            textMesh.anchor = TextAnchor.MiddleLeft;
            textMesh.font = geoTextMesh.font;
            textMesh.fontSize = (int)(geoTextMesh.fontSize * Scale);
            textMesh.text = "";

            MeshRenderer meshRenderer = result.GetComponent<MeshRenderer>();
            meshRenderer.material = textMesh.font.material;
            meshRenderer.enabled = false;

            result.transform.parent = gameObject.transform;
            result.transform.localScale = geoTextObject.transform.localScale;
            result.transform.localPosition = geoTextObject.transform.localPosition;

            return result;
        }

        private Texture2D LoadPNG(string name)
        {
            System.IO.Stream png =
                System.Reflection.Assembly
                    .GetExecutingAssembly()
                    .GetManifestResourceStream($"GrassRandoV2.Resources.{name}");
            try
            {
                byte[] buffer = new byte[png.Length];
                png.Read(buffer, 0, buffer.Length);

                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(buffer, true);
                return texture;
            }
            finally
            {
                png.Dispose();
            }
        }

        private GameObject CreateSpriteObject(string name, string pngName)
        {
            GameObject result = new GameObject(name, typeof(SpriteRenderer));
            result.layer = gameObject.layer;
            UnityEngine.Object.DontDestroyOnLoad(result);

            Texture2D texture = LoadPNG(pngName);
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0, 0.5f), // (0.5, 0.5) is the center of the sprite
                                      // Make the sprite 1 world unit tall
                texture.height);

            SpriteRenderer renderer = result.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 32767;
            renderer.enabled = false;

            GameObject geoTextObject = GetGeoTextObject();
            result.transform.parent = gameObject.transform;
            result.transform.localScale = geoTextObject.transform.localScale;
            result.transform.localPosition = geoTextObject.transform.localPosition;

            // Adjust the height to match the existing geo sprite object multiplied by our scale
            Bounds geoSpriteBounds =
                GetSpriteObject().GetComponent<Renderer>().bounds;
            float targetHeight = geoSpriteBounds.size.y;
            float currentHeight = renderer.bounds.size.y;
            result.transform.localScale *= targetHeight / currentHeight * Scale;

            // Adjust the bottom of the sprite so that it aligns with the
            // bottom of the existing geo sprite
            float targetBottom = geoSpriteBounds.min.y;
            float currentBottom = renderer.bounds.min.y;
            result.transform.localPosition +=
                Vector3.up * (targetBottom - currentBottom);

            return result;
        }

        void Update()
        {
            ReflowLayout();
        }

        static Vector3 WithX(Vector3 v, float newX)
        {
            return new Vector3(newX, v.y, v.z);
        }

        void ReflowLayout()
        {
            if (_layout.Count <= 0)
            {
                return;
            }

            // The first component (the anchor) isn't created by us, and its
            // position is the center of itself (I think), so we use the
            // Renderer's bounds to get its leftmost edge.
            //
            // warn: will probably nre if someone removes the geo counter from our layout,
            // but why the hell would someone do that?
            Transform anchorParentTransform =
                _layout[0].GameObject_!.transform.parent;
            float anchorLeft = anchorParentTransform.InverseTransformPoint(
                _layout[0].GameObject_!.GetComponent<Renderer>().bounds.min).x;

            float currentX = anchorLeft + _layout[0].GetComputedWidth();
            for (int i = 1; i < _layout.Count; ++i)
            {
                Transform transform = _layout[i].GameObject_!.transform;
                transform.localPosition = WithX(transform.localPosition,
                                                currentX);
                currentX += _layout[i].GetComputedWidth();
                _layout[i].GameObject_!.GetComponent<Renderer>().enabled = true;
            }
        }

        GameObject GetGeoTextObject()
        {
            return gameObject.transform.Find("Geo Text").gameObject 
                ?? throw new InvalidOperationException("Cannot find Geo Text.");
        }

        GameObject GetSpriteObject()
        {
            return gameObject.transform.Find("Geo Sprite").gameObject 
                ?? throw new InvalidOperationException("Cannot find Geo Sprite.");
        }

        public void UpdateGrassCount(int newCount)
        {
            if (_globalCount == null) return;
            _globalCount.GetComponent<TextMesh>().text = $"{newCount}";
        }

        public void UpdateLocalGrassCount(int newCount, int totalCount)
        {
            if (_localCount == null) return;
            _localCount.GetComponent<TextMesh>().text = $"{newCount} | {totalCount}";
        }
    }
}
