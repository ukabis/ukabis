<?php

/**
 * @var \App\View\AppView $this
 */
$this->loadHelper('Authentication.Identity');
?>
<?php $this->assign('title', '認証者 閲覧');?>
<article class="contents">
    <section class="title_sec">
        <h2>認証者 閲覧</h2>
    </section>

    <section class="list_sec mb50">
        <table class="table-responsive">
            <tr>
                <th>ID</th>
                <th>氏名</th>
                <th>メールアドレス</th>
                <th>Actions</th>
            </tr>
            <tr>
                <td><?= $checker->id ?></td>
                <td><?= $this->CustomHtml->renderTextWithMaxLength($checker->name) ?></td>
                <td><?= $this->CustomHtml->renderTextWithMaxLength($checker->email, 20) ?></td>
                <td>
                    <?= $this->Html->link('編集', $this->Url->build([
                        'prefix' => 'Checker',
                        'controller' => 'Checker',
                        'action' => 'edit',
                        'id' => $checker->id
                    ], ['fullBase' => true])) ?>
                    <?php if ($user->id != $checker->id): ?>
                        <?= $this->Html->link('削除', $this->Url->build([
                            'prefix' => 'Checker',
                            'controller' => 'Checker',
                            'action' => 'delete',
                            'id' => $checker->id
                        ], ['fullBase' => true])) ?>
                    <?php else: ?>
                        <span></span>
                    <?php endif; ?>
                </td>
            </tr>
        </table>
    </section>

    <section class="btn_sec">
        <p>
            <button class="backBtn">
                <?= $this->Html->link('一覧に戻る', $this->Url->build([
                    'prefix' => 'Checker',
                    'controller' => 'Checker',
                    'action' => 'list',
                ], ['fullBase' => true])) ?>
            </button>
        </p>
    </section>

</article>
