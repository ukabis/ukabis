<?php
declare(strict_types=1);

use Migrations\AbstractMigration;

class CreateUsers extends AbstractMigration
{
    /**
     * Change Method.
     *
     * More information on this method is available here:
     * https://book.cakephp.org/phinx/0/en/migrations.html#the-change-method
     * @return void
     */
    public function change(): void
    {
        $table = $this->table('users', ['id' => false, 'primary_key' => ['id'], 'collation'=>'utf8mb4_unicode_ci']);

        $table->addColumn('id', 'biginteger', [
            'autoIncrement' => true,
            'limit' => 20
        ]);
        $table->addColumn('role', 'integer', [
            'limit' => 2,
            'default' => 1
        ]);
        $table->addColumn('office_id', 'biginteger', [
            'limit' => 20,
            'null' => true,
        ]);
        $table->addColumn('name', 'string', [
            'limit' => 120,
            'null' => false,
        ]);
        $table->addColumn('email', 'string', [
            'limit' => 255,
            'null' => false,
        ]);
        $table->addColumn('password', 'string', [
            'limit' => 255,
            'null' => false,
        ]);
        $table->addColumn('ukabis_mail', 'string', [
            'limit' => 255,
            'null' => true,
        ]);
        $table->addColumn('ukabis_pass', 'string', [
            'limit' => 255,
            'null' => true,
        ]);
        $table->addColumn('verification_code', 'string', [
            'limit' => 15,
            'null' => true,
        ]);
        $table->addColumn('verified_at', 'timestamp', [
            'default' => null,
            'null' => true,
        ]);
        $table->addColumn('created_at', 'timestamp', [
            'default' => 'CURRENT_TIMESTAMP',
            'null' => false,
        ]);
        $table->addColumn('modified_at', 'timestamp', [
            'default' => 'CURRENT_TIMESTAMP',
            'null' => false,
        ]);

        $table->addPrimaryKey("id")
            ->addIndex('email', ['unique' => true]);
        $table->create();
    }
}
