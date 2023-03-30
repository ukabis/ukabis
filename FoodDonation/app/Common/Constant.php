<?php
namespace App\Common;


class Constant
{
    const PRESERVATION_METHOD = [
        '' =>  '選択してください',
        '1' => '常温',
        '2' => '冷蔵',
        '3' => '冷凍',
    ];
    const TRANSPORTATIONMETHOD = [
        '1' => '受け取り',
        '2' => '配送',
        '3' => '未定',
    ];
    const MATCHINGSTATE = [
        '0' => '未マッチ',
        '1' => 'マッチ済み',
        '2' => 'クローズ済み',
    ];

    const TRANSLATE_MAP_1 = [
        'The password cannot contain the first name of the user.'           => 'パスワードには、ユーザーの名を含めることはできません。',
        'The password cannot contain the last name of the user.'            => 'パスワードには、ユーザーの姓を含めることはできません。',
        'The password cannot contain the username of the user.'         => 'パスワードには、ユーザーのユーザー名を含めることはできません。',
        'You are not authorized to perform this action.'         => 'このアクションを実行する権限がありません。',
        'You entered an incorrect user name or password.' => 'メールアドレスまたはパスワードが正しくありません。',//AUTH-3001
        'Your account is locked. Contact your system administrator.' => 'アカウントがロックされています。システム管理者に連絡してください。',//AUTH-3002
        'Your account is deactivated. Contact your system administrator.' => 'アカウントが無効になっています。システム管理者に連絡してください。',//AUTH-3003
        'Your password is expired.' => 'パスワードの有効期限が切れています。',//AUTH-3004
        'You must change your password.' => 'パスワードを変更する必要があります。',//AUTH-3005
    ];
    const TRANSLATE_MAP_2 = [
        '/^The password must have at least (\d+) characters\.$/'                => 'パスワードには少なくとも[1]文字を含める必要があります。',
        '/^The password cannot exceed (\d+) characters\.$/'                     => 'パスワードは[1]文字以下にする必要があります。',
        '/^The password must have at least (\d+) numeric character\.$/'         => 'パスワードには少なくとも[1]個の数字を含める必要があります。',
        '/^The password must have at least (\d+) lowercase character\.$/'            => 'パスワードには少なくとも[1]個の小文字を含める必要があります。',
        '/^The password must have at least (\d+) uppercase character\.$/'            => 'パスワードには少なくとも[1]個の大文字を含める必要があります。',
        '/^The password cannot repeat the most recent (\d+) password\.$/'            => 'パスワードは、直近[1]回のパスワードを再使用することはできません。',
    ];
}