<?php

/**
 * CakePHP(tm) : Rapid Development Framework (https://cakephp.org)
 * Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 *
 * Licensed under The MIT License
 * For full copyright and license information, please see the LICENSE.txt
 * Redistributions of files must retain the above copyright notice.
 *
 * @copyright     Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 * @link          https://cakephp.org CakePHP(tm) Project
 * @since         0.10.0
 * @license       https://opensource.org/licenses/mit-license.php MIT License
 * @var \App\View\AppView $this
 * @var object $params
 */
$appUrl = \Cake\Core\Configure::read('app_url');

echo '事業者名:'. $params->business_name . '<br>
この度は、ふじのくにSDGs認証への申請、誠にありがとうございます。<br>
<br>
厳正な審査の元、申請頂きました内容を確認した結果<br>
事業者名 様の申請内容については以下の結果となりました事<br>
ご報告いたします。<br>
<br>
ーーーーーーーーー<br>
総合得点 ：'. $params->total_score .' 点<br>
認証ステタス : 合格<br>
認証ランク : '. $params->certificate_rank_label .'<br>
ーーーーーーーーー<br>
<br>
つきましては、本メールに記載のご注意点を確認いただき、<br>
以下URLより、ふじのくにSDGs認定証のダウンロードおよび、<br>
認定ステッカーのダウンロードをお願い致します。<br>
<br>
■ふじのくにSDGs認定証<br>
<a href="' . $appUrl . $params->certificate_path .'">' . $appUrl . $params->certificate_path .'</a>
<br>
■ふじのくにSDGs認定ステッカー<br>
<a href="' . $appUrl . $params->mark_path .'">' . $appUrl . $params->mark_path .'</a>   <ご注意点>・本メールお<br>
よびURLによりダウンロード可能な認定証、ステッカーについて<br>
第三者への譲渡および申請施設以外での使用は禁止となります。<br>
・仮に第三者および、申請施設以外での使用が確認された場合は認定の取<br>
り消し等を含めた注意、勧告を致します。<br>
・仮に第三者および、申請施設以外での使用によって生じた問題、紛争につ<br>
いて、本ふじのくに認証制度事務局および静岡県は一切の責任を持ちま<br>
せん。<br>
<br>
ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー<br>'
    . EMAIL_SIGN .
    'ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー';
