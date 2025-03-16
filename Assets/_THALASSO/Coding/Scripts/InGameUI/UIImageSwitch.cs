using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIImageSwitch : MonoBehaviour
{
    [SerializeField]
    private Sprite _defaultImage = default;
    [SerializeField]
    private Sprite[] _imageVariants = default;
    [SerializeField]
    private Color _defaultColor = Color.white;

    private bool _isChangingColor = false;
    private bool _isChangingImage = false;
    private bool _increaseImageIndex = true;
    private int _currentImageIndex = 0;
    private Image _image = default;

    public bool IsDefaultImage => _image.sprite == _defaultImage;
    public Sprite DefaultImage => _defaultImage;
    public Color DefaultColor => _defaultColor;
    public Color CurrentColor => _image.color;

    private void Awake()
    {
        _image = _image != null ? _image : GetComponent<Image>();

        _image.color = _defaultColor;
    }

    private void OnValidate()
    {
        _image = _image != null ? _image : GetComponent<Image>();

        if (_image != null & _defaultImage != null)
        {
            _image.sprite = _defaultImage;
            _image.color = _defaultColor;
        }
    }

    private void Start()
    {
        _image.sprite = _defaultImage;
    }

    public void ChangeColor(Color newColor)
    {
        _image.color = newColor;
    }

    public async void ChangeColorForMilliseconds(Color newColor, int delayTime)
    {
        if (_isChangingColor)
            return;

        _isChangingColor = true;
        Color defaultColor = _image.color;
        ChangeColor(newColor);

        await Task.Delay(delayTime);

        ChangeColor(defaultColor);
        _isChangingColor = false;
    }

    public void SwitchImage()
    {
        if (_increaseImageIndex)
        {
            if (++_currentImageIndex >= _imageVariants.Length - 1)
            {
                _increaseImageIndex = false;
            }
        }
        else
        {
            if (--_currentImageIndex <= 0)
            {
                _increaseImageIndex = true;
            }
        }

        _image.sprite = _imageVariants[_currentImageIndex];
    }

    public async void SwitchImageForMilliseconds(int delayTime)
    {
        if (_isChangingImage)
            return;

        _isChangingImage = true;
        SwitchImage();

        await Task.Delay(delayTime);

        SwitchImage();
        _isChangingImage = false;
    }

    public async void SwitchImageAndChangeColorForMilliseconds(Color newColor, int delayTime)
    {
        if (_isChangingImage || _isChangingColor)
            return;

        _isChangingImage = true;
        _isChangingColor = true;

        Color defaultColor = _image.color;
        SwitchImage();
        ChangeColor(newColor);

        await Task.Delay(delayTime);

        ChangeColor(defaultColor);
        SwitchImage();
        _isChangingImage = false;
        _isChangingColor = false;
    }
}
