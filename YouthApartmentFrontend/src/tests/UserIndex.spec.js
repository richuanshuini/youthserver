import { mount } from '@vue/test-utils';
import UserIndexPage from '../modules/user/pages/Index.vue';

describe('UserIndexPage', () => {
  test('openCreate sets dialog visible and resets form', async () => {
    const wrapper = mount(UserIndexPage, {
      global: {
        // 简化：不渲染 ElementPlus 复杂组件
        stubs: ['el-card','el-form','el-row','el-col','el-form-item','el-input','el-button','el-select','el-option','el-table','el-table-column','el-avatar','el-upload','el-icon','el-switch','el-pagination','el-dialog']
      }
    });

    // 初始值
    expect(wrapper.vm.createDialogVisible).toBe(false);
    expect(wrapper.vm.createForm).toMatchObject({
      userName: '', password: '', email: '', phone: '', realName: '', gender: '', idCard: '', userAvatarUrl: ''
    });

    // 调用并断言
    wrapper.vm.openCreate();
    expect(wrapper.vm.createDialogVisible).toBe(true);
    expect(wrapper.vm.createForm).toMatchObject({
      userName: '', password: '', email: '', phone: '', realName: '', gender: '', idCard: '', userAvatarUrl: ''
    });
  });

  test('openEdit copies row into editForm and opens dialog', async () => {
    const wrapper = mount(UserIndexPage, {
      global: { stubs: ['el-card','el-form','el-row','el-col','el-form-item','el-input','el-button','el-select','el-option','el-table','el-table-column','el-avatar','el-upload','el-icon','el-switch','el-pagination','el-dialog'] }
    });

    const row = {
      userId: 1,
      userName: 'tom',
      password: 'secret',
      email: 'a@b.com',
      phone: '13800000000',
      realName: 'Tom',
      idCard: '110101199001011234',
      gender: '男',
      userAvatarUrl: '/avatars/tom.png',
      status: true
    };

    expect(wrapper.vm.editDialogVisible).toBe(false);
    wrapper.vm.openEdit(row);
    expect(wrapper.vm.editDialogVisible).toBe(true);
    expect(wrapper.vm.editForm).toMatchObject(row);
  });

  test('getErrorMessage extracts messages from various error shapes', async () => {
    const wrapper = mount(UserIndexPage, { global: { stubs: ['el-card','el-dialog'] } });

    const fromValidation = wrapper.vm.getErrorMessage({
      response: { data: { errors: { Email: ['邮箱格式错误'], Phone: ['手机号无效'] } } }
    }, '默认');
    expect(fromValidation).toBe('邮箱格式错误\n手机号无效');

    const fromSimple = wrapper.vm.getErrorMessage({
      response: { data: { error: '简单错误' } }
    }, '默认');
    expect(fromSimple).toBe('简单错误');

    const fromProblem = wrapper.vm.getErrorMessage({
      response: { data: { title: '标题', detail: '细节' } }
    }, '默认');
    expect(fromProblem).toBe('标题：细节');

    const fromDefault = wrapper.vm.getErrorMessage({}, '默认消息');
    expect(fromDefault).toBe('默认消息');
  });

  test('resolveAvatarUrl handles data/http/leading-slash/relative/placeholder', async () => {
    const wrapper = mount(UserIndexPage, { global: { stubs: ['el-card','el-dialog'] } });

    const dataUri = 'data:image/png;base64,abc';
    expect(wrapper.vm.resolveAvatarUrl(dataUri)).toBe(dataUri);

    const httpUrl = 'http://example.com/a.png';
    expect(wrapper.vm.resolveAvatarUrl(httpUrl)).toBe(httpUrl);

    const leading = '/files/a.png';
    expect(wrapper.vm.resolveAvatarUrl(leading)).toBe('http://localhost:5160/files/a.png');

    const relative = 'img/a.png';
    expect(wrapper.vm.resolveAvatarUrl(relative)).toBe('http://localhost:5160/img/a.png');

    expect(wrapper.vm.resolveAvatarUrl('string')).toBe('');
    expect(wrapper.vm.resolveAvatarUrl('')).toBe('');
    expect(wrapper.vm.resolveAvatarUrl(null)).toBe('');
  });
});