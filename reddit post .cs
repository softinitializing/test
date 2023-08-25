 
//
<Page.DataContext>

<Models:BoneNodeViewModel />

</Page.DataContext>

<ScrollViewer HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto">

<Grid x:Name="boneProps">

<TextBox x:Name="TBBoneName" HorizontalAlignment="Left" Margin="132,30,0,0" VerticalAlignment="Top" Width="182" Text="{Binding Bone.Name, UpdateSourceTrigger=LostFocus}" BorderBrush="{x:Null}" Height="22"/>

<TextBox x:Name="TBTranslationX" Margin="113,191,45,0" VerticalAlignment="Top" PreviewTextInput="TBPositionX_PreviewTextInput" Text="{Binding Bone.Translation.X, UpdateSourceTrigger=LostFocus}" BorderBrush="{x:Null}" Height="20" FontFamily="Mongolian Baiti" FontSize="14"/>

<TextBox x:Name="TBTranslationY" Margin="113,211,45,0" VerticalAlignment="Top" PreviewTextInput="TBPositionX_PreviewTextInput" Text="{Binding Bone.Translation.Y, UpdateSourceTrigger=LostFocus}" BorderBrush="{x:Null}" Height="20" FontFamily="Mongolian Baiti" FontSize="14"/>

<TextBox x:Name="TBTranslationZ" Margin="113,231,45,0" VerticalAlignment="Top" PreviewTextInput="TBPositionX_PreviewTextInput" Text="{Binding Bone.Translation.Z, UpdateSourceTrigger=LostFocus}" BorderBrush="{x:Null}" Height="20" FontFamily="Mongolian Baiti" FontSize="14"/>
</Grid>
// the treeView selection event
private void FlverBones_SelectionChanged(object sender, Syncfusion.UI.Xaml.TreeView.ItemSelectionChangedEventArgs e)

{

BoneNodeViewModel selectedNode = (BoneNodeViewModel)FlverBones.SelectedItem;

if (selectedNode == null) { MainWindow.boneProps.IsEnabled = false; return; }

//page instance MainWindow.boneProps

MainWindow.boneProps.DataContext = selectedNode;

}
 
 
 
 
 
 
 //My model and class
 public class BoneNodeViewModel : INotifyPropertyChanged
    {

        private MyBone _bone;
        public MyBone Bone
        {
            get { return _bone; }
            set
            {
                if (_bone != value)
                {
                    if (_bone != null)
                        _bone.PropertyChanged -= MyBone_PropertyChanged;

                    _bone = value;

                    if (_bone != null)
                        _bone.PropertyChanged += MyBone_PropertyChanged;

                    OnPropertyChanged(nameof(_bone));
                }
            }
        }
        private ObservableCollection<BoneNodeViewModel> _children;
        public ObservableCollection<BoneNodeViewModel> Children
        {
            get { return _children; }
            set
            {
                if (_children != value)
                {
                    _children = value;
                    OnPropertyChanged(nameof(Children));
                    OnPropertyChanged(nameof(BoneNames));
                }
            }
        }
        private Matrix3D _parentMatrix = Matrix3D.Identity;
        public Matrix3D ParentMatrix
        {
            get { return _parentMatrix; }
            set
            {
                if (_parentMatrix != value)
                {
                    _parentMatrix = value;
                    OnPropertyChanged(nameof(ParentMatrix));
                }
            }
        }
        private Matrix3D _globalMatrix;
        public Matrix3D GlobalMatrix 
        {
            get { return _globalMatrix; }
            set
            {
                if (_globalMatrix != value)
                {
                    _globalMatrix = value;
                    OnPropertyChanged(nameof(GlobalMatrix));
                }
            }
        }
        
        
        
        
        private BoneDif _boneDif;
        public BoneDif BoneDif
        {
            get { return _boneDif; }
            set
            {
                if (_boneDif != value)
                {
                    if (_boneDif != null)
                        _boneDif.PropertyChanged -= BoneDif_PropertyChanged;

                    _boneDif = value;

                    if (_boneDif != null)
                        _boneDif.PropertyChanged += BoneDif_PropertyChanged;

                    OnPropertyChanged(nameof(BoneDif));
                }
            }
        }
        private void BoneDif_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Handle GeometryDif property changes here
        }
        private string _parentName;
        public string ParentName
        {
            get { return _parentName; }
            set
            {
                if (_parentName != value)
                {
                    _parentName = value;
                    OnPropertyChanged(nameof(ParentName));
                }
            }
        }
        private List<string> _boneNames;
        public List<string> BoneNames
        {
            get { return GetHierarchicalBoneNames(this); }
        }
        private async void MyBone_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MyBone.Translation) ||
                e.PropertyName == nameof(MyBone.Rotation) ||
                e.PropertyName == nameof(MyBone.Scale) ||
                e.PropertyName == nameof(MyBone.ParentIndex))
            {
                CalculateGlobalTransformation();
            }
            
            // ... (other properties)
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public BoneNodeViewModel()
        {
            // Initialize default values or properties here
        }

        public BoneNodeViewModel(MyBone bone)
        {
            Bone = bone;
            ParentName = bone.Parent;
            Children = new ObservableCollection<BoneNodeViewModel>();
            if (bone.ParentIndex == -1)
            {
                ParentMatrix = Matrix3D.Identity;
            }
        }
        public void OnPropertyChanged(string propertyName)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void AddChild(BoneNodeViewModel childNode)
        {
            Children.Add(childNode);
            OnPropertyChanged(nameof(childNode)); // Notify the UI that the Children property has changed
        }

      
        public BoneNodeViewModel FindParentNode(ObservableCollection<BoneNodeViewModel> collection)
        {
            foreach (var node in collection)
            {
                if (node.Bone.Name == ParentName)
                {
                    return node;
                }

                BoneNodeViewModel parent = FindParentNode(node.Children);
                if (parent != null)
                {
                    return parent;
                }
            }

            return null;
        }



        public override string ToString()
        {
            return GetHierarchyString(this, 0);
        }

        private string GetHierarchyString(BoneNodeViewModel node, int level)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(new string(' ', level * 4)); // Indentation based on hierarchy level
            sb.AppendLine(node.Bone.Name);

            foreach (var child in node.Children)
            {
                sb.Append(GetHierarchyString(child, level + 1));
            }

            return sb.ToString();
        }

    }
  public class MyBone
  {

      public string Name;
     
      private string _parent;
      public string Parent
      {
          get { return _parent; }
          set
          {
              if (_parent != value)
              {
                  _parent = value;
                  OnPropertyChanged(nameof(Parent));
              }
          }
      }

      public string Child;
      public string NextSibling;
      public string PreviousSibling;
      public int ParentIndex;
      public int ChildIndex;
      public int NextSiblingIndex;
      public int PreviousSiblingIndex;
      private Vector3 _translation;
      public Vector3 Translation
      {
          get { return _translation; }
          set
          {
              if (_translation != value)
              {
                  _translation = value;
                  OnPropertyChanged(nameof(Translation));
              }
          }
      }
      private Vector3 _rotation;
      public Vector3 Rotation
      {
          get { return _rotation; }
          set
          {
              if (_rotation != value)
              {
                  _rotation = value;
                  OnPropertyChanged(nameof(Rotation));
              }
          }
      }
      private Vector3 _scale;
      public Vector3 Scale
      {
          get { return _scale; }
          set
          {
              if (_scale != value)
              {
                  _scale = value;
                  OnPropertyChanged(nameof(Scale));
              }
          }
      }
      private Vector3 _boundingboxMin;
      public Vector3 BoundingBoxMin
      {
          get { return _boundingboxMin; }
          set
          {
              if (_boundingboxMin != value)
              {
                  _boundingboxMin = value;
                  OnPropertyChanged(nameof(BoundingBoxMin));
              }
          }
      }
      private Vector3 _boundingboxMax;
      public Vector3 BoundingBoxMax
      {
          get { return _boundingboxMax; }
          set
          {
              if (_boundingboxMax != value)
              {
                  _boundingboxMax = value;
                  OnPropertyChanged(nameof(BoundingBoxMax));
              }
          }
      }
      
      public int Unk3C;
      public MyBone()
      {
          Name = "Bone";
          Parent = "SkeletonRoot";
          Child = "Null";
          NextSibling = "Null";
          PreviousSibling = "Null";
          ParentIndex = -1;
          ChildIndex = -1;
          NextSiblingIndex = -1;
          PreviousSiblingIndex = -1;
          Translation = new Vector3(0, 0, 0);
          Rotation = new Vector3(0, 0, 0);
          Scale = new Vector3(1, 1, 1);
          BoundingBoxMin = new Vector3(0, 0, 0);
          BoundingBoxMax = new Vector3(0, 0, 0);
          Unk3C = 0;
      }




      public event PropertyChangedEventHandler PropertyChanged;

      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }


      public override string ToString()
      {
          return Name;
      }

  }