using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[System.Serializable]
public class ItemSelectEvent : UnityEvent<ListView.ListViewItem>
{
}

public class ListView : MonoBehaviour
{
    public UnityAction<ListViewItem> onItemSelected;

    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                onSelected(selected);
            }
        }
        public virtual void onSelected(bool selected)
        {
        }

        public ListView owner;

        public void OnPointerClick(PointerEventData eventData)
        {
            //点击任务项，如果当前项没有被选中，则选中当前项
            if (!this.selected)
            {
                this.Selected = true;
            }
            //如果当前选择的的任务不是自己，则把自己赋值给 owner 的 SelectedItem
            if (owner != null && owner.SelectedItem != this)
            {
                owner.SelectedItem = this;
            }
        }
    }

    List<ListViewItem> items = new List<ListViewItem>();

    private ListViewItem selectedItem = null;
    public ListViewItem SelectedItem
    {
        get { return selectedItem; }
        private set
        {
            //如果当前选中的项不为空，并且新选中的项不是当前选中的项，则取消当前选中项的选中状态
            if (selectedItem!=null && selectedItem != value)
            {
                selectedItem.Selected = false;
            }
            selectedItem = value;
            if (onItemSelected != null)
                onItemSelected.Invoke((ListViewItem)value);
        }
    }

    public void AddItem(ListViewItem item)
    {
        item.owner = this;
        this.items.Add(item);
    }

    public void RemoveAll()
    {
        foreach(var it in items)
        {
            Destroy(it.gameObject);
        }
        items.Clear();
    }

    public void RemoveItem(ListViewItem item)
    {
        Destroy(item.gameObject);
        items.Remove(item);
    }
}
