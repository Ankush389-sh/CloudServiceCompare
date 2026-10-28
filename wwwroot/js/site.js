(function () {
  'use strict';
  var body = document.body;

  function setCookie(name, value) {
    document.cookie = name + '=' + value + ';path=/;max-age=' + (60 * 60 * 24 * 365) + ';samesite=lax';
  }

  /* ---------- highlight active nav link ---------- */
  var path = location.pathname.toLowerCase();
  document.querySelectorAll('.navbar-nav .nav-link').forEach(function (a) {
    var p = a.pathname.toLowerCase();
    var on = p === '/' ? (path === '/' || path.indexOf('/home') === 0) : path.indexOf(p) === 0;
    if (on) a.classList.add('active');
  });

  /* ---------- colour palette switcher (saved in a cookie) ---------- */
  function markDots() {
    var cur = document.documentElement.getAttribute('data-palette');
    document.querySelectorAll('.dot').forEach(function (d) {
      d.classList.toggle('active', d.getAttribute('data-palette') === cur);
    });
  }
  document.querySelectorAll('.dot').forEach(function (d) {
    d.addEventListener('click', function () {
      var p = d.getAttribute('data-palette');
      document.documentElement.setAttribute('data-palette', p);
      setCookie('palette', p);
      markDots();
    });
  });
  markDots();

  /* ---------- dark / light toggle without page reload ---------- */
  var tt = document.getElementById('themeToggle');
  if (tt) {
    tt.addEventListener('click', function (e) {
      e.preventDefault();
      var html = document.documentElement;
      var next = html.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
      html.setAttribute('data-bs-theme', next);
      setCookie('theme', next);
      tt.textContent = next === 'dark' ? '\u2600\uFE0F Light' : '\uD83C\uDF19 Dark';
    });
  }

  /* ---------- auto-hide flash messages ---------- */
  function autoDismiss() {
    document.querySelectorAll('#flash .alert:not([data-timer])').forEach(function (el) {
      el.setAttribute('data-timer', '1');
      setTimeout(function () {
        if (window.bootstrap && document.body.contains(el)) {
          bootstrap.Alert.getOrCreateInstance(el).close();
        }
      }, 4200);
    });
  }
  autoDismiss();

  /* ---------- AJAX "+ Compare" buttons (no page reload) ---------- */
  document.addEventListener('submit', function (e) {
    var f = e.target;
    if (!f.classList || !f.classList.contains('ajax-compare')) return;
    e.preventDefault();

    var btn = f.querySelector('button[type=submit]');
    var old = btn ? btn.textContent : '';
    if (btn) btn.disabled = true;
    body.classList.add('busy');

    fetch(f.action, { method: 'POST', body: new FormData(f), credentials: 'same-origin' })
      .then(function (r) { return r.text(); })
      .then(function (html) {
        var doc = new DOMParser().parseFromString(html, 'text/html');

        var nb = doc.getElementById('cmpBadge'), cb = document.getElementById('cmpBadge');
        if (nb && cb) {
          cb.textContent = nb.textContent;
          cb.classList.remove('pop'); void cb.offsetWidth; cb.classList.add('pop');
        }

        var nf = doc.getElementById('flash'), cf = document.getElementById('flash');
        if (nf && cf) { cf.innerHTML = nf.innerHTML; autoDismiss(); }

        var ok = nf && nf.querySelector('.alert-success');
        if (btn) {
          btn.disabled = false;
          btn.textContent = ok ? '\u2713 Added' : old;
          if (ok) setTimeout(function () { btn.textContent = old; }, 1600);
        }
      })
      .catch(function () { f.submit(); })
      .then(function () { body.classList.remove('busy'); });
  });

  /* ---------- live search / filter on Services page ---------- */
  var form = document.getElementById('filterForm');
  var results = document.getElementById('results');
  if (form && results) {
    var timer, ctrl;
    var run = function () {
      var params = new URLSearchParams(new FormData(form));
      Array.from(params.keys()).forEach(function (k) { if (!params.get(k)) params.delete(k); });
      var qs = params.toString();
      var url = location.pathname + (qs ? '?' + qs : '');

      if (ctrl) ctrl.abort();
      ctrl = new AbortController();
      results.classList.add('loading');
      body.classList.add('busy');

      fetch(url, { signal: ctrl.signal, credentials: 'same-origin' })
        .then(function (r) { return r.text(); })
        .then(function (html) {
          var doc = new DOMParser().parseFromString(html, 'text/html');
          var fresh = doc.getElementById('results');
          if (fresh) results.innerHTML = fresh.innerHTML;
          history.replaceState(null, '', url);
          results.classList.remove('loading');
          body.classList.remove('busy');
        })
        .catch(function (err) {
          if (err.name !== 'AbortError') form.submit();
        });
    };

    form.addEventListener('submit', function (e) { e.preventDefault(); run(); });
    var s = form.querySelector('input[name=search]');
    if (s) s.addEventListener('input', function () { clearTimeout(timer); timer = setTimeout(run, 300); });
    form.querySelectorAll('select').forEach(function (el) { el.addEventListener('change', run); });
  }

  /* ---------- smooth fade-out when clicking internal links ---------- */
  document.addEventListener('click', function (e) {
    if (e.defaultPrevented || e.button !== 0 || e.ctrlKey || e.metaKey || e.shiftKey || e.altKey) return;
    var a = e.target.closest('a[href]');
    if (!a) return;
    if (a.target === '_blank' || a.hasAttribute('download') || a.hasAttribute('data-bs-toggle')) return;
    if (a.origin !== location.origin) return;
    var href = a.getAttribute('href');
    if (!href || href.charAt(0) === '#' || href.indexOf('javascript:') === 0) return;
    e.preventDefault();
    body.classList.add('page-leave');
    setTimeout(function () { window.location.href = a.href; }, 180);
  });

  // show progress bar when a normal form is submitted
  document.addEventListener('submit', function (e) {
    if (!e.defaultPrevented) body.classList.add('busy');
  });

  // back/forward button: clear leaving state
  window.addEventListener('pageshow', function () {
    body.classList.remove('page-leave');
    body.classList.remove('busy');
  });

  /* ---------- count-up numbers ---------- */
  document.querySelectorAll('[data-count]').forEach(function (el) {
    var target = parseInt(el.getAttribute('data-count'), 10) || 0;
    var start = null;
    function step(ts) {
      if (start === null) start = ts;
      var p = Math.min((ts - start) / 700, 1);
      el.textContent = Math.round(target * p);
      if (p < 1) requestAnimationFrame(step);
    }
    requestAnimationFrame(step);
  });
})();
