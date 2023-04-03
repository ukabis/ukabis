<?php
declare(strict_types=1);

use Migrations\AbstractSeed;
use Cake\I18n\FrozenTime;
use Cake\Auth\DefaultPasswordHasher;
use Cake\Datasource\ConnectionManager;

/**
 * Checker seed.
 */
class CheckerSeed extends AbstractSeed
{
    /**
     * Run Method.
     *
     * Write your database seeder using this method.
     *
     * More information on writing seeds is available here:
     * https://book.cakephp.org/phinx/0/en/seeding.html
     *
     * @return void
     */
    public function run(): void
    {
        $connection = ConnectionManager::get('default')->newQuery();
        $count = $connection->select('id')
            ->from('users')
            ->where(['email =' => 'dev_shizuoka@dev.com'])
            ->execute()->count();

        if (!$count) {
            $datetime = (new FrozenTime())->format('Y-m-d H:m:s');
            $data = [
                [
                    'name' => 'dev_shizuoka',
                    'email' => 'dev_shizuoka@dev.com',
                    'password' => (new DefaultPasswordHasher())->hash('1356Abcd'),
                    'role' => ROLE_CHECKER,
                    'created_at' => $datetime,
                    'modified_at' => $datetime,
                ]
            ];

            $table = $this->table('users');
            $table->insert($data)->save();
        }
    }
}
