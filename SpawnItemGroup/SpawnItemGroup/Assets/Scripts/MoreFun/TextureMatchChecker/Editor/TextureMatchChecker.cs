using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Killer.Tools
{


    public class TextureMatchChecker : EditorWindow
    {

        private Object[] SpriteFolds;
        private Object[] PrefabFolds;
        private Object[] IgnorePrefabFolds;
        private string numSpriteFile = "1";
        private string numPrefabsFile = "1";
        private string numIgnoreFile = "0";
        private static MatchChecherWorker m_Worker;
        private int spriteSize = 0;
        private int prefabSize = 0;
        private int ignorSize = 0;
        private Vector2 vs1;
        private Vector2 vs2;
        [MenuItem("Killer/重复贴图检查")]
        static void Init()
        {
            m_Worker = new MatchChecherWorker();
            TextureMatchChecker window = (TextureMatchChecker)EditorWindow.GetWindow(typeof(TextureMatchChecker));
            window.minSize = new Vector2(500, 500);
        }

        void OnGUI()
        {
            Color orColor = GUI.color;

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("搜索Sprite"))
            {
                m_Worker.BeginReadSprite(SpriteFolds);
            }
            if (m_Worker.isCanMatchSprite())
            {
                GUILayout.Space(10);
                if (GUILayout.Button("搜索Prefab"))
                {
                    m_Worker.BeginReadPrefab(PrefabFolds, IgnorePrefabFolds);
                }
            }
            if (m_Worker.isCanMatchSprite())
            {
                GUILayout.Space(10);
                if (GUILayout.Button("匹配Sprite"))
                {
                    m_Worker.DoMatchSprite();
                }
            }

            if (m_Worker.isCanMatchPrefab())
            {
                GUILayout.Space(10);

                if (GUILayout.Button("关联Sprite与Prefab"))
                {
                    m_Worker.MathAll();
                }
            }
            if(m_Worker.CanDeleteSprite)
            {
                GUILayout.Space(10);

                if (GUILayout.Button("删除无引用Sprite"))
                {
                    m_Worker.DeleteNullSprite();
                }
                GUILayout.Space(10);

                if (GUILayout.Button("剔除重复Sprite"))
                {
                    m_Worker.DeleteSameSprite(false);
                }
                GUILayout.Space(10);

                if (GUILayout.Button("全部干掉"))
                {
                    m_Worker.DeleteSameSprite(true);
                }
            }
            GUILayout.Space(10);

            if (GUILayout.Button("清空"))
            {
                m_Worker.Clear();
            }
            GUILayout.EndHorizontal();
            GUI.color = Color.green;

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            vs1 = GUILayout.BeginScrollView(vs1);
            int size = 1;
            numSpriteFile = GUILayout.TextField(numSpriteFile, GUILayout.Width(50));
            if (numSpriteFile != null && numSpriteFile.Length > 0)
            {
                if (!int.TryParse(numSpriteFile, out size))
                {
                    size = 1;
                }
            }
            if(spriteSize != size)
            {
                SpriteFolds = new Object[size];
                spriteSize = size;
            }
            for (int i = 0; i < spriteSize; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("选择 Sprite 所在的目录 : ", GUILayout.Width(200));
                GUILayout.Space(10);
                SpriteFolds[i] = EditorGUILayout.ObjectField(SpriteFolds[i], typeof(Object), false);
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("共 " + m_Worker.getSpriteSize() + " 张Sprite");
            GUILayout.EndHorizontal();
            //显示搜索
            GUILayout.Space(20);
            List<SameSprite> daos = m_Worker.m_SameSprites;
            if (daos.Count > 0)
            {
                GUI.color = Color.white;
                GUILayout.Label("Sprite匹配结果！ 重复的Spite有： " + daos.Count + "张！");
                GUI.color = Color.yellow;
                GUILayout.Space(10);
                for (int i = 0; i < daos.Count; i++)
                {
                    GUILayout.Label(i + " -->: 相同的Sprite Path");
                    SameSprite dao = daos[i];
                    for (int j = 0; j < dao.m_Sprites.Count; j++)
                    {
                        GUILayout.Label("--------Sprite Path : " + dao.m_Sprites[j].m_path + "  ,引用数 ： " + dao.m_Sprites[j].count);
                    }
                    GUILayout.Space(5);
                }
            }else
            {
                GUILayout.Label("无重复贴图?！");
            }

            GUILayout.Space(30);
            GUI.color = Color.blue;

            if(m_Worker.showNullSprite)
            {
                GUILayout.Label(" -------------引用数为 0 的Sprite ， 建议清空！");
                GUILayout.Space(10);
                int num = 0;
                List<SpriteDao> SpriteDaos = m_Worker.m_Sprites;
                for(int i = 0 ; i < SpriteDaos.Count; i ++)
                {
                    if(SpriteDaos[i].count == 0)
                    {
                        num++;
                        GUILayout.Label("Path : " + SpriteDaos[i].m_path);
                        GUILayout.Space(2);
                    }
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.Space(50);
            GUILayout.BeginVertical();
            vs2 = GUILayout.BeginScrollView(vs2);
            GUI.color = Color.red;
            int size1 = 1;
            numPrefabsFile = GUILayout.TextField(numPrefabsFile, GUILayout.Width(50));
            if (numPrefabsFile != null && numPrefabsFile.Length > 0)
            {
                if (!int.TryParse(numPrefabsFile, out size1))
                {
                    size1 = 1;
                }
            }
            if (prefabSize != size1)
            {
                PrefabFolds = new Object[size1];
                prefabSize = size1;
            }
            for (int i = 0; i < prefabSize; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("选择 Prefab 所在的目录 : ", GUILayout.Width(200));
                GUILayout.Space(10);
                PrefabFolds[i] = EditorGUILayout.ObjectField(PrefabFolds[i], typeof(Object), false);
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
            }
            GUILayout.Space(10);
            GUI.color = Color.cyan;
            int size2 = 1;
            numIgnoreFile = GUILayout.TextField(numIgnoreFile, GUILayout.Width(50));
            if (numIgnoreFile != null && numIgnoreFile.Length > 0)
            {
                if (!int.TryParse(numIgnoreFile, out size2))
                {
                    size2 = 1;
                }
            }
            if (ignorSize != size2)
            {
                IgnorePrefabFolds = new Object[size2];
                ignorSize = size2;
            }
            for (int i = 0; i < ignorSize; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("忽略Prefab 所在的目录 : ", GUILayout.Width(200));
                GUILayout.Space(10);
                IgnorePrefabFolds[i] = EditorGUILayout.ObjectField(IgnorePrefabFolds[i], typeof(Object), false);
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
            }

            GUILayout.Space(20);

            List<PrefabDao> pdaos = m_Worker.m_Prefabs;
            if (pdaos.Count > 0)
            {
                GUI.color = Color.white;
                GUILayout.Label("Prafab 搜索结果 ，包含了Sprite索引的Prefab有 " + pdaos.Count + " 个！");
                GUILayout.Space(10);
                GUI.color = Color.yellow;
                for (int i = 0; i < pdaos.Count; i++)
                {
                    PrefabDao pd = pdaos[i];
                    GUILayout.Label("   Sprite Path " + pd.m_Path + "  , 包含的Sprite UID ： ");
                    for (int j = 0; j < pd.lines.Count; j++)
                    {
                        string msg = j + " --------Sprite ID : " + pd.lines[j].m_Uid;
                        int ind = pd.lines[j].SpriteId;
                        if(ind != -1)
                        {
                            string SpriP = m_Worker.m_Sprites[ind].m_path;
                            msg += " Sprite Path ：" + SpriP;
                        }
                        GUILayout.Label(msg);
                    }
                    GUILayout.Space(5);
                }
            }
            else
            {
                GUILayout.Label("没发现包含了Sprite的Prefab!");
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }


    }

    public class MatchChecherWorker
    {
        public bool CanDeleteSprite = false;//用来判断是否可以在界面中显示相应的按钮操作
        public bool ShowFileName = false;
        public bool showNullSprite = false;
        private MD5 m_md5 = MD5.Create();//处理MD5
        public List<SpriteDao> m_Sprites;//存储查找到的全部Sprite数据
        public List<PrefabDao> m_Prefabs;//存储查找到的全部Prefab数据
        public List<SameSprite> m_SameSprites;//存储相同的Sprite数据
        public Dictionary<string, int> spritePathTry = new Dictionary<string, int>();//用来保存读取过的文件路径，避免重复读取
        public Dictionary<string, int> prefabPathTry = new Dictionary<string, int>();
        public Dictionary<string, int> spriteUidFindIndex = new Dictionary<string, int>();//根据Sprite的guid来标记对应的Sprite是哪一个
        //private int assetPathStartIndex = 0;
        private Dictionary<string, int> mSpritesDic = new Dictionary<string, int>();//做合并的中间字典
        public MatchChecherWorker()
        {
            m_Sprites = new List<SpriteDao>();
            m_Prefabs = new List<PrefabDao>();
            m_SameSprites = new List<SameSprite>();
        }

        /// <summary>
        /// 将msg保存到name的文件中
        /// </summary>
        /// <param name="name"></param>
        /// <param name="msg"></param>
        private void SaveFile(string name,string msg)
        {
            StreamWriter files = File.CreateText(name);
            files.Write(msg);
            files.Close();
        }
        /// <summary>
        /// 根据输入的路径集读取相应的Sprite
        /// </summary>
        /// <param name="objs"></param>
        public void BeginReadSprite(Object[] objs)
        {
            m_Sprites.Clear();
            DirectoryInfo initDir;
            string pathDir = Application.dataPath;
            //assetPathStartIndex = pathDir.Length - 6;
            spriteUidFindIndex.Clear();
            spritePathTry.Clear();
            int size = objs.Length;
            for (int i = 0; i < size; i++)
            {
                Object textureFold = objs[i];
                if (textureFold != null)
                {
                    //转化为文件结构
                    initDir = new DirectoryInfo(pathDir + AssetDatabase.GetAssetPath(textureFold).Substring(6));
                    //收索SPrite文件存放到m_Sprites结构中
                    ListTextureFiles(initDir, m_Sprites);
                }
            }
        }


        //检验相同的Sprite结构，这一步有点绕，自己仔细想一下就好了
        public void DoMatchSprite()
        {
            m_SameSprites.Clear();
            mSpritesDic.Clear();
            Dictionary<int, int> mSames = new Dictionary<int, int>();//用来存放已经检索到的SameSprite
            int count = m_Sprites.Count;
            for (int i = 0; i < count; i++)//逐个检验SpriteDao
            {
                SpriteDao dao = m_Sprites[i];
                if (mSpritesDic.ContainsKey(dao.m_fileMd5))//判断之前的MD5码是否已经存在
                {
                    int index = mSpritesDic[dao.m_fileMd5];
                    if (mSames.ContainsKey(index))//如果之前已经存在了
                    {
                        m_SameSprites[mSames[index]].m_Sprites.Add(dao);//将当前的SpriteDao添加在先前的m_SameSprites中
                    }
                    else
                    {
                        SameSprite sdao = new SameSprite();//如果不存在则添加一个新的
                        sdao.m_Sprites.Add(m_Sprites[index]);//把相同的两个都添加
                        sdao.m_Sprites.Add(dao);
                        m_SameSprites.Add(sdao);
                        mSames.Add(index, m_SameSprites.Count - 1);//存到中间检验的集合中
                    }
                }
                else
                {
                    mSpritesDic.Add(dao.m_fileMd5, i);//如果不存在则添加新的md5
                }
            }
            SaveFile("ReadSprite.txt", saveReadSprite());
        }
        /// <summary>
        /// 将当前读取到的Sprite保存到文件中
        /// </summary>
        /// <returns></returns>
        private string saveReadSprite()
        {
            string msg = "";
            msg += ("共 " + getSpriteSize() + " 张Sprite!") + "\r\n";
            msg += ("Sprite匹配结果！ 重复的Spite有： " + m_SameSprites.Count + "张！") + "\r\n";
            for (int i = 0; i < m_SameSprites.Count; i++)
            {
                msg += (i + " -->: 相同的Sprite Path") + "\r\n";
                for (int j = 0; j < m_SameSprites[i].m_Sprites.Count; j++)
                {
                    msg += ("--------Sprite Path : " + m_SameSprites[i].m_Sprites[j].m_path + "  ,引用数 ： " + m_SameSprites[i].m_Sprites[j].count) + "\r\n";
                }
            }
            msg += (" -------------引用数为 0 的Sprite ， 建议清空！") + "\r\n";
            for (int i = 0; i < m_Sprites.Count; i++)
            {
                if (m_Sprites[i].count == 0)
                {
                    msg += ("Path : " + m_Sprites[i].m_path) + "\r\n";
                }
            }
            return msg;
        }
        /// <summary>
        /// 在info文件中收索符合的Sprite
        /// </summary>
        /// <param name="info"></param>
        /// <param name="fileList"></param>
        public void ListTextureFiles(FileSystemInfo info, List<SpriteDao> fileList)
        {
            if (!info.Exists) return;
            DirectoryInfo dir = info as DirectoryInfo;
            if (dir == null || spritePathTry.ContainsKey(dir.FullName)) return;//如果当前的文件路径已经读取过了，则返回

            spritePathTry.Add(dir.FullName, 0);//标记已经读取过了
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)//逐个迭代
            {
                FileInfo file = files[i] as FileInfo;
                if (file != null)
                {
                    string finalName = file.FullName;
                    finalName = finalName.Replace('\\', '/');//字符替换
                    if (finalName.EndsWith(".png.meta") || finalName.EndsWith(".jpg.meta"))//这里只是判断png跟jpg的图片，可以再添加，用meta后缀主要是fileId跟guid需要在meta中读取，如果没有meta文件那么这个图片就是没意义的
                    {
                        SpriteDao dao = CreateSpriteDao(finalName);//根据文件创建相应的SpriteDao
                        if (null != dao)
                        {
                            fileList.Add(dao);//添加到SpriteDao集合中
                            if (!spriteUidFindIndex.ContainsKey(dao.m_Uid))//将当前的FileId保存到字典用，方便取用
                            {
                                spriteUidFindIndex.Add(dao.m_Uid, fileList.Count - 1);
                            }
                        }
                    }
                }
                else
                    ListTextureFiles(files[i], fileList);//如果是文件夹则继续迭代
            }
        }

       /// <summary>
       /// 根据Path路径判断是否是Sprite。如果不是则返回null
       /// </summary>
       /// <param name="path"></param>
       /// <returns></returns>
        private SpriteDao CreateSpriteDao(string path)
        {
            SpriteDao dao = new SpriteDao();
            string[] strs = File.ReadAllLines(path);//读取该meta文件的内容
            if (!isSprite(strs))//根据内容判断是否是Sprite
            {
                return null;
            }
            if (strs.Length >= 1)
            {
                dao.m_Uid = strs[1].Replace("guid: ", "");//取到Sprite相应的Guid，这里这样做是应为Sprite的guid都是保存在第2行中，其实应该是用正则表达式判定比较正确
            }
            string pathFile = path.Replace(".meta", "");
            dao.m_path = pathFile;//真正的文件路径
            byte[] msgs = File.ReadAllBytes(pathFile);//根据字节内容提取MD5
            if (msgs != null)
            {
                dao.m_fileLenght = msgs.Length;
                dao.m_fileMd5 = GetMd5Hash(m_md5, msgs);
            }
            return dao;
        }

        //判断是否是Sprite
        private bool isSprite(string []strs)
        {
            for (int i = strs.Length - 1; i >= 0; i--)
            {
                if (strs[i].Contains("textureType: 8"))//Sprite的标记 8 
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 根据input内容哈希出MD5
        /// </summary>
        /// <param name="md5Hash"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string GetMd5Hash(MD5 md5Hash, byte[] input)
        {

            byte[] data = md5Hash.ComputeHash(input);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 读取Prefab文件
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="ignors"></param>
        public void BeginReadPrefab(Object[] objs,Object []ignors)
        {
            m_Prefabs.Clear();
            DirectoryInfo initDir;
            string pathDir = Application.dataPath;
            //assetPathStartIndex = pathDir.Length - 6;
            prefabPathTry.Clear();
                
            //将不需要检查的文件路径先保存到字典中
            for(int i = 0 ; ignors != null && i < ignors.Length; i++ )
            {
                if (ignors[i] != null)
                {
                    initDir = new DirectoryInfo(pathDir + AssetDatabase.GetAssetPath(ignors[i]).Substring(6));
                    if (!prefabPathTry.ContainsKey(initDir.FullName))
                    {
                        prefabPathTry.Add(initDir.FullName, 0);
                    }
                }
            }
    
            int size = objs.Length;
            for (int i = 0; i < size; i++)
            {
                Object textureFold = objs[i];
                if (textureFold != null)
                {
                    initDir = new DirectoryInfo(pathDir + AssetDatabase.GetAssetPath(textureFold).Substring(6));
                    //迭代所有文件李静
                    ListPrefabFiles(initDir, m_Prefabs);
                }
            }
            //将收索到的数据保存到文本中
            SaveFile("ReadPrefab.txt", SaveReadPrefab());
        }

        /// <summary>
        /// 保存读取到的prefab信息
        /// </summary>
        /// <returns></returns>
        private string SaveReadPrefab()
        {
            string msgs = "";
            List<PrefabDao> pdaos = m_Prefabs;
            if (pdaos.Count > 0)
            {
                msgs += ("Prafab 搜索结果 ，包含了Sprite索引的Prefab有 " + pdaos.Count + " 个！") + "\r\n";
                for (int i = 0; i < pdaos.Count; i++)
                {
                    PrefabDao pd = pdaos[i];
                    msgs += ("   Sprite Path " + pd.m_Path + "  , 包含的Sprite UID ： ") + "\r\n";
                    for (int j = 0; j < pd.lines.Count; j++)
                    {
                        string msg = j + " --------Sprite ID : " + pd.lines[j].m_Uid;
                        int ind = pd.lines[j].SpriteId;
                        if (ind != -1)
                        {
                            string SpriP = m_Sprites[ind].m_path;
                            msg += " Sprite Path ：" + SpriP;
                        }
                        msgs += (msg) + "\r\n";
                    }
                }
            }
            else
            {
                msgs += ("没发现包含了Sprite的Prefab!") + "\r\n";
            }
            return msgs;
        }

        /// <summary>
        /// 将info中的prefab（包含了Sprite）保存到fileList中
        /// </summary>
        /// <param name="info"></param>
        /// <param name="fileList"></param>
        public void ListPrefabFiles(FileSystemInfo info, List<PrefabDao> fileList)
        {
            if (!info.Exists) return;
            DirectoryInfo dir = info as DirectoryInfo;
            if (dir == null || prefabPathTry.ContainsKey(dir.FullName)) return;//判断是否收索过该路径

            prefabPathTry.Add(dir.FullName, 0);
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;
                if (file != null)
                {
                    string finalName = file.FullName;
                    finalName = finalName.Replace('\\', '/');
                    if (finalName.EndsWith(".prefab") || finalName.EndsWith(".mat") || finalName.EndsWith(".unity"))//判定是否是prefab或者是shader文件，似乎只有这两个才会引用到Sprite，其实场景也可以添加进来的
                    {
                        PrefabDao dao = CreatePrefabDao(finalName);//根据路径创建PrefabDao，如果找不到Sprite则会返回null
                        if (null != dao)
                            fileList.Add(dao);
                    }
                }
                else
                    ListPrefabFiles(files[i], fileList);//如果是文件则继续迭代
            }
        }

        /// <summary>
        /// 根据路径创建PrefabDao，如果找不到Sprite则会返回null
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private PrefabDao CreatePrefabDao(string path)
        {
            PrefabDao dao = null;
            string[] strs = File.ReadAllLines(path);
            int count = strs.Length;
            for (int i = 0; i < count; i++)
            {
                //Regex regex = new Regex(@".*Sprite: \{fileID: ([0-9]+)?, guid: ([0-9a-z]+).+$");
                Regex regex = new Regex(@".*\{fileID: ([0-9]+)?, guid: ([0-9a-z]+).+$");//用正则表达式判断该行是否存在了相应的字段，提取fileid于guid
                Match match = regex.Match(strs[i]);
                if (match.Success)//如果存在
                {
                    if (spriteUidFindIndex.ContainsKey(match.Groups[2].Value))//判断之前是的sprite的Guid集合中是否存在该guid，如果不存在则表示该文件不是sprite
                    {
                        if (dao == null)
                        {
                            dao = new PrefabDao();
                            dao.m_Path = path;
                        }
                        LineDao ld = new LineDao();//prefab中保存了该文件中引用到的所有Sprite信息
                        ld.m_line = i;
                        ld.LineMsg = strs[i];//该行的内容，方便后面做替换操作
                        ld.m_Uid = match.Groups[2].Value;//guid的值
                        ld.fileId = match.Groups[1].Value;//fileId的值
                        ld.SpriteId = spriteUidFindIndex[ld.m_Uid];//根据Guid查找对应的SpriteDao在集合中的位置
                        dao.lines.Add(ld);
                    }
                }
            }
            return dao;
        }

        /// <summary>
        /// 能否校验相同的Sprite，必须先检索过Sprite
        /// </summary>
        /// <returns></returns>
        public bool isCanMatchSprite()
        {
            return m_Sprites.Count > 0;
        }

        //是否能关联Prefab跟Sprite
        public bool isCanMatchPrefab()
        {
            return m_Sprites.Count > 0 && m_Prefabs.Count > 0;
        }

        public int getSpriteSize()
        {
            return m_Sprites.Count;
        }

        public int getPrefabSize()
        {
            return m_Prefabs.Count;
        }

        /// <summary>
        /// prefab与Sprite关联
        /// </summary>
        public void MathAll()
        {
            int count = m_Prefabs.Count;
            for (int i = 0; i < count; i++)
            {
                FindPrefab(m_Prefabs[i], i);//根据Prefab中保存的Sprite信息，将该Prefab的路径添加到Sprite的引用中。
            }


            count = m_Sprites.Count;
            for (int i = 0; i < count; i++)
            {
                m_Sprites[i].count = m_Sprites[i].m_prefabs.Count;//重新统计SPrite的引用数
            }
            CanDeleteSprite = true;
            showNullSprite = true;
            //将关联后的信息保存到文本中
            SaveFile("MathAll.txt", saveReadSprite() + "\r\n\r\n " + SaveReadPrefab());
        }
        /// <summary>
        /// 根据Prefab中保存的Sprite信息，将该Prefab的路径添加到Sprite的引用中。
        /// 恩，这个命名好，请帮改之
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="id"></param>
        private void FindPrefab(PrefabDao dao, int id)
        {
            List<LineDao> daos = dao.lines;//查看当前的prefab都引用了那些sprite文件
            for (int i = 0; i < daos.Count; i++)
            {

                int index = daos[i].SpriteId;//找到对应的spriteDao
                if (-1 == index)//如果之前index==-1，有可能是先收索了Sprite。这个是之前版本的判断，现在不会出现这个情况了
                {
                    if (spriteUidFindIndex.ContainsKey(daos[i].m_Uid))
                        index = daos[i].SpriteId = spriteUidFindIndex[daos[i].m_Uid];//重现查找Sprite对应的位置
                }
                if (index != -1)
                {
                    m_Sprites[index].m_prefabs.Add(id);//将Prefab在集合中的位置添加到引用中
                    m_Sprites[index].setFileId(daos[i].fileId);//替换改sprite的fileId，只取一个有用的
                }
                else
                {
#if DEV_BUILD
                    Debug.LogError("Can Find Id");
#endif
                }
            }
        }
        //删除引用数为0的Sprite
        public void DeleteNullSprite()
        {
            int count = m_Sprites.Count;
            for (int i = 0; i < count; i++)
            {
                if (0 == m_Sprites[i].count )//如果引用数为0
                {
                    deleteFile(m_Sprites[i]);
                }
            }
            Clear();
        }
        /// <summary>
        /// 删除相同的SPrite，并且做prefab引用的强替换
        /// </summary>
        /// <param name="DeleteNULL">是否将引用数为0的Sprite也一并删除</param>
        public void DeleteSameSprite(bool DeleteNULL)
        {
            int count = m_SameSprites.Count;
            for (int i = 0; i < count; i++ )
            {
                ClearSameSprite(m_SameSprites[i]);//迭代每一个相同的Sprite集合做删除
            }
            count = m_Prefabs.Count;
            for (int i = 0; i < count; i++ )
            {
                m_Prefabs[i].ReCopy();//修正prefab的资源中的内容
            }
            if (DeleteNULL)
            {
                DeleteNullSprite();//删除引用为0的资源
            }
            Clear();
        }

        /// <summary>
        /// 清空相同的sprite，替换prefab中的引用
        /// </summary>
        /// <param name="Sdao">包含了那些相同的sprite</param>
        private void ClearSameSprite(SameSprite Sdao)
        {
            List<SpriteDao> daos = Sdao.m_Sprites;//所有具有相同MD5的Sprite
            int count = daos.Count;
            if (count > 1)//只有引用数不为0才用意义。其实也必定是大于0的
            {
                string mUId = null;//取一个正确的guid跟fileId做prefab的替换
                string fileId = null;
                int defauleIndex = -1;

                for (int i = 0; i < count; i++)
                {
                    if (daos[i].count != 0)//引用不为0
                    {
                        if(daos[i].getFileId().IndexOf("213") == 0)//如果是以213开头的，213开头是Sprite的fileId，如果不是则可能是shader中的引用（280000，不能使用）
                        {
                            fileId = daos[i].getFileId();//取到该fileId
                            mUId = daos[i].m_Uid;
                            defauleIndex = i;//标记该Sprite不能被删除
                            break;
                        }
                    }
                }

                if(defauleIndex == -1)//如果没有找到
                {
                    mUId = daos[0].m_Uid;
                    fileId = "21300000";//默认值
                }


                for (int i = 0; i < count; i++)
                {
                    if (daos[i].count == 0)//如果没有被引用，直接删除
                    {
                        deleteFile(daos[i]);
                    }
                    else
                    {
                        if(defauleIndex != i)//如果是
                        {
                            ReplacePrefab(mUId, fileId, daos[i]);//替换Sprite中的中的guid跟fileId
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 替换prefab中对sprite的引用
        /// </summary>
        /// <param name="m_Uid">当前可用的guid</param>
        /// <param name="fileId">当前可用的fileId</param>
        /// <param name="dao"></param>
        private void ReplacePrefab(string m_Uid,string fileId, SpriteDao dao)
        {
            List<int> ids = dao.m_prefabs;//找到当前sprite都被那些prefab/mat 文件引用
            if (ids.Count > 0)
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    m_Prefabs[ids[i]].ReplaceUID(m_Uid,fileId, dao);//逐个进行替换
                    dao.count--;//减掉当前sprite的引用数
                }
            }
            if (dao.count == 0)//似乎这个判断是多余的
            {
                deleteFile(dao);//如果没有引用了。则删除
            }
        }
        public void Clear()//清空现有数据
        {
            CanDeleteSprite = false;
            ShowFileName = false;
            m_Sprites.Clear();
            m_Prefabs.Clear();
            m_SameSprites.Clear();
            spritePathTry.Clear();
            prefabPathTry.Clear();
            spriteUidFindIndex.Clear();
            mSpritesDic.Clear();
            showNullSprite = false;
            AssetDatabase.Refresh();

        }

        private void deleteFile(SpriteDao dao)//删除资源文件
        {
            try
            {
                if (null != dao && !dao.isDelete)//如果没有被删除的话
                {
                    File.Delete(dao.m_path);
                    File.Delete(dao.m_path + ".meta");
                    dao.isDelete = true;
                }
            }
            catch (System.Exception e) { }
        }
    }

    /// <summary>
    /// 用来记录每一个Prefab中对应的信息，判断是否引用了其他的Sprite
    /// </summary>
    public class PrefabDao
    {
        public string m_Path;//资源的绝对路径
        public List<LineDao> lines = new List<LineDao>();//用来记录所有出现了SpriteId的行号
        private bool isUpdate = false;
        public void ReplaceUID(string UID, string fileId,SpriteDao dao)
        {
            for(int i = 0 ; i < lines.Count ;i++)
            {
                if (lines[i].m_Uid.Equals(dao.m_Uid))//如果该行的guid引用相同，则替换
                {
                    lines[i].LineMsg = lines[i].LineMsg.Replace(dao.m_Uid, UID);//首先替换guid
                    if (!lines[i].fileId.Equals("2800000"))//似乎sharder里面引用到的sprite的guid只会是2800000
                        lines[i].LineMsg = lines[i].LineMsg.Replace(lines[i].fileId, fileId);//替换fileId
                    isUpdate = true;//标记已经替换
                }
            }
        }

        public void ReCopy()//将修改后的内容重新填充回到
        {
            if (!isUpdate)//如果没有被修改。则不需要
                return;

            string[] strs = File.ReadAllLines(m_Path);//先读取，
            for (int i = 0; i < lines.Count; i++ )//所有被修改的行
            {
                strs[lines[i].m_line] = lines[i].LineMsg;//对相应的行进行替换
            }
            File.WriteAllLines(m_Path, strs);
            isUpdate = false;
        }

    }
    /// <summary>
    /// 用来标记Prefab文件信息中哪一行被扣出了Sprite的信息
    /// </summary>
    public class LineDao
    {
        public int m_line;//行号
        public string LineMsg;//行内容
        public string m_Uid;//Sprite的id
        public int SpriteId = -1;
        public string fileId;

        //public string fileId
        //{
        //    get
        //    {
        //        return m_fileId;
        //    }
        //    set//如果大于才设置
        //    {
        //        int id = int.Parse(m_fileId);
        //        int id_ = int.Parse(fileId);
        //        if(id_ > id)
        //        {
        //            m_fileId = fileId;
        //        }
        //    }
        //}

    }

    public class SpriteDao
    {
        public string m_path;//Sprite文件的绝对路径
        public string m_Uid;//Sprite的Id；
        public long m_fileLenght;//Sprite文件长度
        public string m_fileMd5;//Sprite文件的Md5内容
        public List<int> m_prefabs = new List<int>();//记录Sprite被引用的prefab列表
        public int count = -1;
        public bool isDelete = false;
        private string m_fileId = "21300000";//默认fileId

        public string getFileId()
        {
            return m_fileId;
        }

        public void setFileId(string fileId)
        {
            if(fileId.IndexOf("213") == 0)//如果是已213开头的,如果是280000开头则不理
            {
                int indexF = int.Parse(m_fileId);
                int indexN = int.Parse(fileId);
                if (indexN > indexF)//只有大的才需要替换，目前是这样的
                {
                    m_fileId = fileId;
                }
            }
        }

    }
    /// <summary>
    /// 标记相同的Sprite
    /// </summary>
    public class SameSprite
    {
        public long m_fileLenght;
        public string m_fileMd5;
        public List<SpriteDao> m_Sprites = new List<SpriteDao>();//存储了相同SpriteDao的列表
    }
}
