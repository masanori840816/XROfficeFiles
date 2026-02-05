using System;
using UnityEngine;

namespace XROfficeFiles.Pages
{
    public interface IPageBehaviour
    {
        void OpenPage();
        void ClosePage();
        Action<AppPage> MoveNextPage { set; }
    }
}