// ----------------------------------------------------------------------------
// <copyright file="ITortoiseSVNSettingWindow.cs" company="上海序曲网络科技有限公司">
// Copyright (C) 2015 上海序曲网络科技有限公司
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited 
// without the prior written consent of the copyright owner.
// </copyright>
// <author>HuHuiBin</author>
// <date>12/03/2016</date>
// ----------------------------------------------------------------------------
namespace SeanLib.ThridPartyTool.Tools.TortoiseSVN.Editor
{
    public interface ITortoiseSVNSettingEditor
    {
        void ShowTortoiseSVNSetting();

        int GetTortoiseSVNSettingOrder();
    }
}