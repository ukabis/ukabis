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
 */

$class = 'shnsei-page';
$currentUrl = $this->Url->build(null, ['fullBase' => true]);

//remove query
if (str_contains($currentUrl, '?')) {
    $currentUrl = strstr($this->Url->build(null, ['fullBase' => true]), "?", true);
}

switch ($currentUrl) {
    case $this->Url->build(["prefix" => "Restaurants", "controller" => "Auth", "action" => "login"], ['fullBase' => true]):
        $class = 'shnsei-page';
        $title = '飲食店申請者トップページ';
        break;
    case $this->Url->build(["prefix" => "Farms", "controller" => "Auth", "action" => "login"], ['fullBase' => true]):
        $class = 'shnsei-page farm_top';
        $title = '生産者申請者トップページ';
        break;
    default:
        $class = '';
        break;
}

?>
<!DOCTYPE html>
<html lang="ja">
<head>
    <?= $this->Html->charset() ?>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title><?= $title ?></title>
    <?= $this->Html->meta('icon') ?>
    <?= $this->Html->meta('csrfToken', $this->request->getAttribute('csrfToken')); ?>

    <link href="https://fonts.googleapis.com/css?family=Raleway:400,700" rel="stylesheet">
    <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.15.4/css/all.css">
    <?= $this->Html->css(['style']) ?>

    <?= $this->Html->script("plugins/jquery.min") ?>
    <?= $this->Html->script("plugins/fontawesome.all") ?>

    <?= $this->fetch('meta') ?>
    <?= $this->fetch('css') ?>
    <?= $this->fetch('script') ?>
</head>
<body class="common_page <?= $class ?>">
    <main>
        <div class="wrap_page">
            <?= $this->Flash->render() ?>
            <?= $this->fetch('content') ?>
        </div>
        <?= $this->element("user/common_footer"); ?>
    </main>

    <?= $this->Html->script("common") ?>
</body>
</html>
