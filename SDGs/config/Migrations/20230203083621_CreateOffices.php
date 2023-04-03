<?php
declare(strict_types=1);

use Migrations\AbstractMigration;

class CreateOffices extends AbstractMigration
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
        $table = $this->table('offices', ['id' => false, 'primary_key' => ['id'], 'collation'=>'utf8mb4_unicode_ci']);

        $table->addColumn('id', 'biginteger', [
            'autoIncrement' => true,
            'limit' => 20
        ]);
        $table->addColumn('business_id', 'biginteger', [
            'limit' => 20,
            'null' => false,
        ]);
        $table->addColumn('office_type', 'integer', [
            'limit' => 2,
            'default' => 1
        ]);
        $table->addColumn('name', 'string', [
            'limit' => 255,
            'null' => false,
        ]);
        $table->addColumn('name_kana', 'string', [
            'limit' => 255,
            'null' => false,
        ]);
        $table->addColumn('zip_code', 'string', [
            'limit' => 20,
            'null' => false,
        ]);
        $table->addColumn('prefecture', 'string', [
            'limit' => 255,
            'null' => false,
        ]);
        $table->addColumn('city', 'string', [
            'limit' => 255,
            'null' => false,
        ]);
        $table->addColumn('street', 'string', [
            'limit' => 255,
            'null' => true,
        ]);
        $table->addColumn('tel', 'string', [
            'limit' => 20,
            'null' => true,
        ]);
        $table->addColumn('fax', 'string', [
            'limit' => 20,
            'null' => true,
        ]);
        $table->addColumn('offices_introduction', 'text', [
            'null' => true,
        ]);
        $table->addColumn('offices_pr_message', 'string', [
            'limit' => 255,
            'null' => true,
        ]);
        $table->addColumn('offices_certified_rank', 'string', [
            'limit' => 255,
            'null' => true,
        ]);
        $table->addColumn('offices_certified_file_path', 'string', [
            'default' => null,
            'limit' => 255,
            'null' => true,
        ]);
        $table->addColumn('profile_image_1', 'string', [
            'limit' => 255,
            'null' => true,
        ]);
        $table->addColumn('profile_image_2', 'string', [
            'limit' => 255,
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

        $table->addPrimaryKey("id");
        $table->create();
    }
}
