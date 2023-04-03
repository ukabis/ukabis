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
?>
<!DOCTYPE html>
<html lang="ja">
<head>
    <?= $this->Html->charset() ?>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title><?= $this->fetch('title') ?></title>
    <?= $this->Html->meta('icon') ?>
    <?= $this->Html->meta('csrfToken', $this->request->getAttribute('csrfToken')); ?>

    <link href="https://fonts.googleapis.com/css?family=Raleway:400,700" rel="stylesheet">
    <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.15.4/css/all.css">
    <?= $this->Html->css(['style']) ?>
    <?= $this->Html->css(['plugins/jquery.toast.min']) ?>
    <?= $this->Html->css(['plugins/viewer']) ?>

    <?= $this->Html->script("plugins/jquery.min") ?>
    <?= $this->Html->script("plugins/fontawesome.all") ?>
    <?= $this->Html->script("plugins/browser-image-compression") ?>
    <?= $this->Html->script("plugins/jquery.toast.min") ?>
    <?= $this->Html->script("plugins/loadingoverlay.min") ?>
    <?= $this->Html->script("plugins/viewer.min") ?>
    <?= $this->fetch('meta') ?>
    <?= $this->fetch('css') ?>
    <?= $this->fetch('script') ?>
</head>
<body class="farm_page">
    <main>
        <div class="wrap_page">
            <?= $this->element('user/farm/farm_header'); ?>
            <?= $this->Flash->render() ?>
            <?= $this->fetch('content') ?>
        </div>
        <?= $this->Form->hidden("env", ['value' => $env ]) ?>
        <?= $this->Form->hidden("office_id", ['value' => !empty($user) ? $user->office_id : '' ]) ?>
        <?= $this->element('user/farm/farm_footer'); ?>
    </main>

    <?= $this->Html->script("common") ?>
    <?= $this->Html->script("farm") ?>
    <?= $this->fetch("farm_application") ?>
</body>
</html>
