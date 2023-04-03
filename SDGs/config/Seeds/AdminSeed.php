<?php
declare(strict_types=1);

use Cake\Auth\DefaultPasswordHasher;
use Cake\Datasource\ConnectionManager;
use Migrations\AbstractSeed;

/**
 * Admin seed.
 */
class AdminSeed extends AbstractSeed
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
            ->where(['email =' => 'dev_admin@admin.com'])
            ->execute()->count();

        if (!$count) {
            $data = [
                [
                    'name' => 'dev_admin',
                    'email' => 'dev_admin@admin.com',
                    'password' => (new DefaultPasswordHasher())->hash('Qaws1234+'),
                    'role' => ROLE_ADMIN
                ]
            ];

            $table = $this->table('users');
            $table->insert($data)->save();
        }
    }
}
