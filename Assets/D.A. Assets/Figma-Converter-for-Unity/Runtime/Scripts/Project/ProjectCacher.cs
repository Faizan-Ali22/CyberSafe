using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class ProjectCacher : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        internal void Cache<T>(T @object)
        {
            try
            {
                FigmaProject figmaProject = (FigmaProject)Convert.ChangeType(@object, typeof(FigmaProject));

                string projectUrl = monoBeh.Settings.MainSettings.ProjectUrl;

                RecentProject projectCache = new RecentProject
                {
                    Url = projectUrl,
                    Name = figmaProject.Name,
                    DateTime = DateTime.Now
                };

                List<RecentProject> cachedProjects = GetRecentProjects();
                cachedProjects.RemoveAll(pc => pc.Url == projectUrl);
                cachedProjects.Insert(0, projectCache);

                if (cachedProjects.Count > FcuConfig.RecentProjectsLimit)
                {
                    cachedProjects = cachedProjects.Take(FcuConfig.RecentProjectsLimit).ToList();
                }

                SaveAll(cachedProjects);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public List<RecentProject> GetRecentProjects()
        {
#if UNITY_EDITOR
            string savedData = UnityEditor.EditorPrefs.GetString(FcuConfig.RECENT_PROJECTS_PREFS_KEY, "");

            if (savedData.IsEmpty())
            {
                return new List<RecentProject>();
            }

            List<RecentProject> cachedProjects = DAJson.FromJson<List<RecentProject>>(savedData);

            if (!cachedProjects.IsEmpty())
            {
                return cachedProjects.OrderByDescending(x => x.DateTime).ToList();
            }
            else
            {
                return new List<RecentProject>();
            }
#else
            return new List<RecentProject>();
#endif
        }

        private void SaveAll(List<RecentProject> cachedProjects)
        {
            if (cachedProjects == null)
            {
                cachedProjects = new List<RecentProject>();
            }

            string json = DAJson.ToJson(cachedProjects);
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetString(FcuConfig.RECENT_PROJECTS_PREFS_KEY, json);
#endif
        }
    }


    [Serializable]
    public struct RecentProject
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
    }
}