// ========================================================
// Autor：Zhaori 
// CreateTime：2019/08/21 15:08:43 
// Des：游戏工具类
// ========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using Core;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace IL.Game
{
    public static class GameUtils
    {
        
        /// <summary>
        /// 概率返回true
        /// </summary>
        /// <param name="percent">0~100</param>
        /// <returns></returns>
        public static bool Probability(float percent)
        {
            float r = UnityEngine.Random.Range(0, 100f);
            if (r <= percent)
            {
                return true;
            }

            return false;
        }
        

        /// <summary>
        /// 遍历父物体得到子物体
        /// </summary>
        /// <param name="parent">父物体</param>
        /// <param name="targetName">子物体名字</param>
        /// <returns></returns>
        public static Transform GetTransform(Transform parent, string targetName)
        {
            Transform tempTrans = null;
            foreach (Transform child in parent)
            {
                if (child.name == targetName)
                {
                    return child;
                }
                else
                {
                    tempTrans = GetTransform(child, targetName);
                    if (tempTrans != null)
                    {
                        return tempTrans;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获得最近的游戏物体
        /// </summary>
        /// <param name="targetList"></param>
        /// <param name="TargetGo"></param>
        /// <returns></returns>
        public static GameObject GetNearestGo(List<GameObject> targetList, GameObject TargetGo)
        {
            if (targetList.Count == 0)
            {
                return null;
            }

            float distance = Vector3.Distance(targetList[0].transform.position, TargetGo.transform.position);
            GameObject temp = targetList[0];
            for (int i = 0; i < targetList.Count; i++)
            {
                float dis = Vector3.Distance(targetList[i].transform.position, TargetGo.transform.position);
                if (dis <= distance)
                {
                    distance = dis;
                    temp = targetList[i];
                }
            }

            return temp;
        }

        /// <summary>
        /// 获得最近的游戏物体
        /// </summary>
        /// <param name="targetList"></param>
        /// <param name="TargetGo"></param>
        /// <returns></returns>
        public static GameObject GetNearestGo(Collider2D[] targetList, GameObject TargetGo)
        {
            if (targetList.Length == 0)
            {
                return null;
            }

            float distance = Vector3.Distance(targetList[0].transform.position, TargetGo.transform.position);
            Collider2D temp = null;


            for (int i = 0; i < targetList.Length; i++)
            {
                if (targetList[i].gameObject == TargetGo)
                {
                    continue;
                }

                if (temp == null)
                {
                    temp = targetList[i];
                }

                float dis = Vector3.Distance(targetList[i].transform.position, TargetGo.transform.position);
                if (dis <= distance)
                {
                    distance = dis;
                    temp = targetList[i];
                }
            }

            if (temp == null)
            {
                return null;
            }

            return temp.gameObject;
        }

        /// <summary>
        /// 获得最远的游戏物体
        /// </summary>
        /// <param name="targetList"></param>
        /// <param name="TargetGo"></param>
        /// <returns></returns>
        public static GameObject GetFatherGo(List<GameObject> targetList, GameObject TargetGo)
        {
            float distance = 0;
            GameObject temp = null;
            for (int i = 0; i < targetList.Count; i++)
            {
                float dis = Vector3.Distance(targetList[i].transform.position, TargetGo.transform.position);
                if (dis >= distance)
                {
                    Debug.LogError(dis + targetList[i].name);
                    distance = dis;
                    temp = targetList[i];
                }
            }

            return temp;
        }


        //
        // Vector2.Distance(a.transform.position, b.transform.position):
        //    _____        _____
        //   |     |      |     |
        //   |  x==|======|==x  |
        //   |_____|      |_____|
        //
        //
        // Utils.ClosestDistance(a.collider, b.collider):
        //    _____        _____
        //   |     |      |     |
        //   |     |x====x|     |
        //   |_____|      |_____|
        //
        //public static float ClosestDistance(Collider2D a, Collider2D b)
        //{
        //    return Vector2.Distance(a.ClosestPointOnBounds(b.transform.position),
        //                            b.ClosestPointOnBounds(a.transform.position));
        //}

        public static Vector2 ClosestPointOnBounds(this Collider2D collider, Vector2 position)
        {
            return collider.bounds.ClosestPoint(position);
        }

        /// <summary>
        /// 打开指定路径的文件夹（可以使用文件路径，可以使用相对路径）
        /// </summary>
        public static void OpenFolder(string path)
        {
#if UNITY_EDITOR_WIN
            path = path.Replace('/', '\\');

            while (!Directory.Exists(path))
                path = path.Substring(0, path.LastIndexOf('\\'));

            System.Diagnostics.Process.Start("explorer.exe", path);
#endif
        }

        /// <summary>
        /// 字符串转数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static T[] JsoonConvert2Array<T>(string data, string seperator = ",")
        {
            return Array.ConvertAll(data.Split(new string[1] {seperator}, StringSplitOptions.RemoveEmptyEntries),
                s => JsonUtil.FromJson<T>(s));
        }

        /// <summary>
        /// 字符串转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static List<T> JsonConvert2List<T>(string data, string seperator = ",")
        {
            List<string> list = new List<string>();
            list.AddRange(data.Split(new string[1] {seperator}, StringSplitOptions.RemoveEmptyEntries));
            return list.ConvertAll(s => JsonUtil.FromJson<T>(s));
        }

        /// <summary>
        /// 字符串分割
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static List<string> Convert2List(string data, string seperator = ",")
        {
            if (string.IsNullOrEmpty(data))
            {
                Debug.LogError("被分割的字符串为空");
                return null;
            }

            List<string> list = new List<string>();
            list.AddRange(data.Split(new string[1] {seperator}, StringSplitOptions.RemoveEmptyEntries));
            return list;
        }

        /// <summary>
        /// 字符串分割
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static List<int> Convert2ListInt(string data, string seperator = ",")
        {
            List<int> list = new List<int>();

            foreach (var item in data.Split(new string[1] {seperator}, StringSplitOptions.RemoveEmptyEntries))
            {
                list.Add(int.Parse(item));
            }

            return list;
        }

        /// <summary>
        /// 在集合中随机多个元素
        /// 会破坏原集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="count"></param>
        /// <param name="isRepeat"></param>
        /// <returns></returns>
        public static List<T> RandomMultiBreak<T>(IList<T> t, int count, bool isRepeat = false)
        {
            List<T> result = new List<T>();
            for (int i = 0; i < count; i++)
            {
                int seed = UnityEngine.Random.Range(0, t.Count);
                result.Add(t[seed]);
                if (!isRepeat)
                    t.RemoveAt(seed);
            }

            return result;
        }

        /// <summary>
        /// 在集合中随机多个元素
        /// 不会破坏原集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<T> RandomMulti<T>(IList<T> t, int count, bool isRepeat = false)
        {
            List<T> list = new List<T>();
            List<int> ids = new List<int>(t.Count);
            for (int i = 0; i < t.Count; i++)
                ids.Add(i);
            ids = RandomMultiBreak(ids, count, false);
            for (int i = 0; i < ids.Count; i++)
                list.Add(t[ids[i]]);
            return list;
        }

        /// <summary>
        /// 随机一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        public static T Random<T>(IList<T> t, bool remove = false)
        {
            int seed = Random(0, t.Count - 1);
            T result = t[seed];
            if (remove)
                t.RemoveAt(seed);
            return result;
        }

        /// <summary>
        /// 修改字体颜色
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string TextColor(object obj, Color col)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<color=#");
            string temp;
            temp = Convert.ToString((int) (col.r * 255), 16);
            if (temp.Length <= 1)
                temp = "0" + temp;
            builder.Append(temp);
            temp = Convert.ToString((int) (col.g * 255), 16);
            if (temp.Length <= 1)
                temp = "0" + temp;
            builder.Append(temp);
            temp = Convert.ToString((int) (col.b * 255), 16);
            if (temp.Length <= 1)
                temp = "0" + temp;
            builder.Append(temp);
            builder.Append(">");
            builder.Append(obj);
            builder.Append("</color>");
            return builder.ToString();
        }

        /// <summary>
        /// 修改字体颜色,如绿色(col参数为00FF00)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string TextColor(object obj, string col)
        {
            return "<color=#" + col + ">" + obj + "</color>";
        }

        /// <summary>
        /// 倒序排列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public static void SortInvert<T>(IList<T> t)
        {
            T item;
            int index;
            for (int i = 0; i < t.Count / 2; i++)
            {
                item = t[i];
                index = t.Count - 1 - i;
                t[i] = t[index];
                t[index] = item;
            }
        }

        /// <summary>
        /// 随机一个数包含最小值和最大值
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Random(int min, int max, bool open = true)
        {
            if (open)
                return UnityEngine.Random.Range(min, max);
            return UnityEngine.Random.Range(min, max + 1);
        }

        /// <summary>
        /// 随机一个小数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// 将小树转换为百分数
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static string DecimalToPercent(float f, bool round = false, int count = 0)
        {
            f = f * 100;
            if (round)
                f = (float) Math.Round(f, count);
            else
                f = (int) f;
            return f.ToString("f" + count) + "%";
        }

        /// <summary>
        /// 取指定位数的小数
        /// </summary>
        /// <param name="val"></param>
        /// <param name="digit"></param>
        /// <param name="round"></param>
        /// <returns></returns>
        public static float RetainDecimal(float val, int digit = 2, bool round = false)
        {
            if (round && val > 1) digit += 1;
            var format = (round ? "G" : "F") + digit;
            val = float.Parse(val.ToString(format));
            return val;
        }

        /// <summary>
        /// string数组转int数组
        /// </summary>
        /// <param name="vas"></param>
        /// <returns></returns>
        public static List<int> StringToInt(List<string> vas)
        {
            List<int> s = new List<int>();
            foreach (var item in vas)
            {
                try
                {
                    int p = int.Parse(item);
                    s.Add(p);
                }
                catch (Exception e)
                {
                    Debug.LogError("格式错误" + e);
                }
            }

            return s;
        }


        /// <summary>
        /// 根据 秒 转换时分秒
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string PrettySeconds(float seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            string res = "";
            if (t.Days > 0) res += t.Days + "d";
            if (t.Hours > 0) res += " " + t.Hours + "h";
            if (t.Minutes > 0) res += " " + t.Minutes + "m";
            if (t.Milliseconds > 0) res += " " + t.Seconds + "." + (t.Milliseconds / 100) + "s";
            else if (t.Seconds > 0) res += " " + t.Seconds + "s";
            return res != "" ? res : "0s";
        }


        /// <summary>
        /// 两个手指滑动距离
        /// </summary>
        /// <returns></returns>
        // source: https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
        public static float GetPinch()
        {
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // 每帧获得长度
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                return touchDeltaMag - prevTouchDeltaMag;
            }

            return 0;
        }

        /// <summary>
        /// 获得最后一个大写的单词 例如
        ///   EquipmentWeaponBow => Bow
        ///   EquipmentShield => Shield
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ParseLastNoun(string text)
        {
            MatchCollection matches = new Regex(@"([A-Z][a-z]*)").Matches(text);
            return matches.Count > 0 ? matches[matches.Count - 1].Value : "";
        }

        /// <summary>
        /// 判断是否点击在UI上
        /// </summary>
        /// <returns></returns>
        public static bool IsCursorOverUserInterface()
        {
            // 鼠标左键
            if (EventSystem.current.IsPointerOverGameObject())
                return true;
            // Touches
            for (int i = 0; i < Input.touchCount; ++i)
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                    return true;
            // 在GUI上
            return GUIUtility.hotControl != 0;
        }

        /// <summary>
        /// 概率
        /// </summary>
        /// <param name="probabilities"></param>
        /// <returns></returns>
        public static int Probability(List<Double> probabilities)
        {
            double[] _probability = new double[probabilities.Count];
            //对应的下标
            int[] _alias = new int[probabilities.Count];
            //平均值
            double average = 1.0 / probabilities.Count;
            //大小栈
            var small = new Stack<int>();
            var large = new Stack<int>();

            for (int i = 0; i < probabilities.Count; ++i)
            {
                if (probabilities[i] >= average)
                    large.Push(i);
                else
                    small.Push(i);
            }

            while (small.Count > 0 && large.Count > 0)
            {
                int less = small.Pop();
                int more = large.Pop();

                _probability[less] = probabilities[less] * probabilities.Count;
                _alias[less] = more;

                probabilities[more] = (probabilities[more] + probabilities[less] - average);

                if (probabilities[more] >= average)
                    large.Push(more);
                else
                    small.Push(more);
            }

            while (small.Count > 0)
                _probability[small.Pop()] = 1.0;
            while (large.Count > 0)
                _probability[large.Pop()] = 1.0;

            return next(_probability, _alias);
        }

        public static int next(double[] probability, int[] alias)
        {
            long tick = DateTime.Now.Ticks;
            var seed = ((int) (tick & 0xffffffffL) | (int) (tick >> 32));
            unchecked
            {
                seed = (seed + Guid.NewGuid().GetHashCode() + new Random().Next(0, 100));
            }

            var random = new Random(seed);
            int column = random.Next(probability.Length);

            bool coinToss = random.NextDouble() < probability[column];

            return coinToss ? column : alias[column];
        }

        

        /// <summary>
        /// 对相机拍摄区域进行截图，如果需要多个相机，可类比添加，可截取多个相机的叠加画面
        /// </summary>
        /// <param name="camera">待截图的相机</param>
        /// <param name="width">截取的图片宽度</param>
        /// <param name="height">截取的图片高度</param>
        /// <param name="fileName">文件名</param>
        /// <returns>返回Texture2D对象</returns>
        public static Texture2D CameraCapture(Camera camera, Rect rect, string fileName)
        {
            Debug.Log("rect.x:" + rect.x + "rect.y:" + rect.y);
            // RenderTexture render = new RenderTexture((int) Math.Floor(rect.width), (int) Math.Floor(rect.height), -1);//创建一个RenderTexture对象 

            // camera.gameObject.SetActive(true);//启用截图相机

            // camera.targetTexture = render;//设置截图相机的targetTexture为render
            // camera.Render();//手动开启截图相机的渲染

            // RenderTexture.active = render;//激活RenderTexture
            Texture2D tex =
                new Texture2D((int) rect.width, (int) rect.height, TextureFormat.ARGB32, false); //新建一个Texture2D对象
            tex.ReadPixels(rect, 0, 0); //读取像素
            tex.Apply(); //保存像素信息

            // camera.targetTexture = null;//重置截图相机的targetTexture
            // RenderTexture.active = null;//关闭RenderTexture的激活状态
            // Object.Destroy(render);//删除RenderTexture对象

            byte[] bytes = tex.EncodeToPNG(); //将纹理数据，转化成一个png图片
            System.IO.File.WriteAllBytes(fileName, bytes); //写入数据
            Debug.Log(string.Format("截取了一张图片: {0}", fileName));

#if UNITY_EDITOR
            // UnityEditor.AssetDatabase.Refresh();//刷新Unity的资产目录
#endif

            return tex; //返回Texture2D对象，方便游戏内展示和使用
        }
    }


    // public class Capture
    // {
    //     public Texture2D tex;
    //
    //     public void Jie()
    //     {
    //         ApplicationKit.Ins.StartCoroutine(CameraCapt());
    //     }
    //     
    //     IEnumerator CameraCapt()
    //     {
    //         yield return new WaitForEndOfFrame();
    //         Camera camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    //         Rect rect=new Rect();
    //         rect.x = UnityEngine.Screen.width;
    //         rect.y = UnityEngine.Screen.height;
    //         tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);//新建一个Texture2D对象
    //         tex.ReadPixels(rect, 0, 0);//读取像素
    //         tex.Apply();//保存像素信息
    //         
    //         Debug.Log("完成");
    //     }
    // }

    public class MyButton
    {
        //正在被按住
        public bool IsPressing = false;

        //刚刚被按住 触发
        public bool OnPressed = false;

        //刚刚被释放
        public bool OnReleased = false;

        //双击
        public bool IsExtending = false;

        //长按
        public bool IsDelaying = false;

        public float extendingDuration = 0.3f;
        public float delayingDuration = 0.3f;
        bool curState = false;
        bool lastState = false;

        MyTimer extTimer = new MyTimer();
        MyTimer delayTimer = new MyTimer();


        public void Tick(bool input)
        {
            extTimer.Tick();
            delayTimer.Tick();

            curState = input;
            IsPressing = curState;

            OnPressed = false;
            OnReleased = false;
            IsExtending = false;
            IsDelaying = false;
            if (curState != lastState)
            {
                if (curState)
                {
                    OnPressed = true;
                    StartTimer(delayTimer, delayingDuration);
                }
                else
                {
                    OnReleased = true;
                    StartTimer(extTimer, extendingDuration);
                }
            }

            lastState = curState;
            if (extTimer.state == MyTimer.STATE.RUN)
            {
                IsExtending = true;
            }

            if (delayTimer.state == MyTimer.STATE.RUN)
            {
                IsDelaying = true;
            }


            void StartTimer(MyTimer timer, float duration)
            {
                timer.duration = duration;
                timer.GO();
            }
        }
    }

    public class MyTimer
    {
        //事件
        public float duration = 1.0f;

        //时间流失
        float elapsedTime = 0;

        public enum STATE
        {
            IDLE,
            RUN,
            FINISHED
        }

        public STATE state;

        public void Tick()
        {
            switch (state)
            {
                case STATE.IDLE:
                    break;
                case STATE.RUN:
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime >= duration)
                    {
                        state = STATE.FINISHED;
                    }

                    break;
                case STATE.FINISHED:
                    break;
                default:
                    break;
            }
        }

        public void GO()
        {
            elapsedTime = 0;
            state = STATE.RUN;
        }
    }


    public static class Clone
    {
        /// <summary>
        /// 深克隆
        /// </summary>
        /// <param name="obj">原始版本对象</param>
        /// <returns>深克隆后的对象</returns>
        public static object DepthClone(this object obj)
        {
            object clone = new object();
            using (Stream stream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(stream, obj);
                    stream.Seek(0, SeekOrigin.Begin);
                    clone = formatter.Deserialize(stream);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
            }

            return clone;
        }
    }
}