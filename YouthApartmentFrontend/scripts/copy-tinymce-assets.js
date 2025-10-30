/* eslint-env node */
// Copy TinyMCE assets (plugins/themes/icons) from node_modules to public/ticymce
// Run: node scripts/copy-tinymce-assets.js
import fs from 'fs/promises';
import path from 'path';
import process from 'node:process';

const projectRoot = path.resolve(process.cwd());
const nmTiny = path.join(projectRoot, 'node_modules', 'tinymce');
const pubTiny = path.join(projectRoot, 'public', 'ticymce');

async function ensureDir(dir) {
  try {
    await fs.mkdir(dir, { recursive: true });
  } catch (err) {
    // 忽略已存在目录；其余错误抛出
    if (err && err.code !== 'EEXIST') throw err;
  }
}

async function copyDir(src, dest) {
  await ensureDir(dest);
  const entries = await fs.readdir(src, { withFileTypes: true });
  for (const entry of entries) {
    const s = path.join(src, entry.name);
    const d = path.join(dest, entry.name);
    if (entry.isDirectory()) {
      await copyDir(s, d);
    } else if (entry.isFile()) {
      await fs.copyFile(s, d);
    }
  }
}

async function main() {
  // Validate source exists
  try {
    await fs.access(nmTiny);
  } catch {
    console.error('tinymce not found in node_modules. Please run `npm install` first.');
    process.exit(1);
  }

  await ensureDir(pubTiny);

  // Copy core file if missing (keep existing if present)
  try {
    await fs.copyFile(path.join(nmTiny, 'tinymce.min.js'), path.join(pubTiny, 'tinymce.min.js'));
  } catch {
    // 已存在或复制失败：保持现有文件
    void 0;
  }

  // Copy skins
  await copyDir(path.join(nmTiny, 'skins'), path.join(pubTiny, 'skins'));

  // Copy themes (silver)
  await copyDir(path.join(nmTiny, 'themes'), path.join(pubTiny, 'themes'));

  // Copy icons (default)
  await copyDir(path.join(nmTiny, 'icons'), path.join(pubTiny, 'icons'));

  // Copy plugins
  await copyDir(path.join(nmTiny, 'plugins'), path.join(pubTiny, 'plugins'));

  // Copy models (e.g., dom)
  await copyDir(path.join(nmTiny, 'models'), path.join(pubTiny, 'models'));

  console.log('TinyMCE assets copied to public/ticymce successfully.');
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});