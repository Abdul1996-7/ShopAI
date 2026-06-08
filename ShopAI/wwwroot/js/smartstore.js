(function () {
  const initialized = {
    quickView: false,
    tableFilter: false,
    cart: false
  };

  const CART_STORAGE_KEY = 'smartstore.cart.v1';

  function createLucideIcons(root) {
    if (window.lucide && typeof window.lucide.createIcons === 'function') {
      window.lucide.createIcons({}, root || document.body);
    }
  }

  function initAos() {
    if (!window.AOS) {
      return;
    }

    if (!document.body.dataset.aosReady) {
      window.AOS.init({
        once: true,
        duration: 650,
        easing: 'ease-out-cubic',
        offset: 70
      });
      document.body.dataset.aosReady = 'true';
    } else {
      window.AOS.refreshHard();
    }
  }

  function initHeroSwiper(root) {
    if (!window.Swiper) {
      return;
    }

    root.querySelectorAll('[data-swiper="hero"]').forEach((element) => {
      if (element.dataset.swiperReady) {
        return;
      }

      element.dataset.swiperReady = 'true';
      new window.Swiper(element, {
        loop: true,
        speed: 900,
        effect: 'slide',
        autoplay: {
          delay: 5200,
          disableOnInteraction: false
        },
        pagination: {
          el: element.querySelector('.swiper-pagination'),
          clickable: true
        }
      });
    });
  }

  function initProductSwipers(root) {
    if (!window.Swiper) {
      return;
    }

    root.querySelectorAll('[data-swiper="products"]').forEach((element) => {
      if (element.dataset.swiperReady) {
        return;
      }

      const section = element.closest('section');
      element.dataset.swiperReady = 'true';
      new window.Swiper(element, {
        slidesPerView: 1.08,
        spaceBetween: 18,
        speed: 650,
        navigation: {
          nextEl: section?.querySelector('.carousel-next'),
          prevEl: section?.querySelector('.carousel-prev')
        },
        breakpoints: {
          640: { slidesPerView: 2.1 },
          1024: { slidesPerView: 3.1 },
          1280: { slidesPerView: 4 }
        }
      });
    });

    root.querySelectorAll('[data-swiper="product-thumbs"]').forEach((element) => {
      if (element.dataset.swiperReady) {
        return;
      }

      element.dataset.swiperReady = 'true';
      new window.Swiper(element, {
        slidesPerView: 3.2,
        spaceBetween: 12,
        breakpoints: {
          640: { slidesPerView: 4.5 },
          1024: { slidesPerView: 5.5 }
        }
      });
    });
  }

  function initProductGallery(root) {
    if (!window.lightGallery) {
      return;
    }

    root.querySelectorAll('[data-lightgallery]').forEach((element) => {
      if (element.dataset.galleryReady) {
        return;
      }

      element.dataset.galleryReady = 'true';
      window.lightGallery(element, {
        selector: '[data-gallery-item]',
        plugins: [window.lgZoom, window.lgThumbnail].filter(Boolean),
        speed: 420,
        download: false,
        counter: true,
        actualSize: false
      });
    });

    root.querySelectorAll('[data-open-gallery]').forEach((button) => {
      if (button.dataset.galleryTriggerReady) {
        return;
      }

      button.dataset.galleryTriggerReady = 'true';
      button.addEventListener('click', () => {
        button.closest('[data-lightgallery]')?.querySelector('[data-gallery-item]')?.click();
      });
    });
  }

  function initProductThumbs(root) {
    root.querySelectorAll('[data-product-thumb]').forEach((button) => {
      if (button.dataset.thumbReady) {
        return;
      }

      button.dataset.thumbReady = 'true';
      button.addEventListener('click', () => {
        const imageUrl = button.dataset.productThumb;
        const image = document.getElementById('main-product-image');
        if (!image || !imageUrl) {
          return;
        }

        image.src = imageUrl;
        const link = image.closest('a');
        if (link) {
          link.href = imageUrl;
        }
      });
    });
  }

  function initTomSelect(root) {
    if (!window.TomSelect) {
      return;
    }

    root.querySelectorAll('[data-tom-select]').forEach((select) => {
      if (select.tomselect) {
        return;
      }

      const instance = new window.TomSelect(select, {
        create: false,
        allowEmptyOption: true,
        maxOptions: 100,
        plugins: ['clear_button'],
        render: {
          no_results: () => '<div class="no-results px-3 py-2 text-sm text-slate-500">لا توجد نتائج</div>'
        }
      });

      instance.wrapper.classList.add('smartstore-select');
    });
  }

  function initQuickView() {
    if (initialized.quickView) {
      return;
    }

    initialized.quickView = true;
    const modal = document.querySelector('[data-quick-view-modal]');
    if (!modal) {
      return;
    }

    const closeModal = () => {
      modal.classList.add('hidden');
      modal.classList.remove('flex');
      document.body.classList.remove('quick-view-open');
    };

    document.addEventListener('click', (event) => {
      const trigger = event.target.closest('[data-quick-view]');
      if (!trigger) {
        return;
      }

      modal.querySelector('[data-quick-view-image]').src = trigger.dataset.image || '/images/product-placeholder.svg';
      modal.querySelector('[data-quick-view-title]').textContent = trigger.dataset.title || '';
      modal.querySelector('[data-quick-view-price]').textContent = trigger.dataset.price || '';
      modal.querySelector('[data-quick-view-category]').textContent = trigger.dataset.category || '';
      modal.querySelector('[data-quick-view-condition]').textContent = trigger.dataset.condition || '';
      modal.querySelector('[data-quick-view-store]').textContent = trigger.dataset.store || '';
      modal.querySelector('[data-quick-view-link]').href = trigger.dataset.url || '#';
      modal.classList.remove('hidden');
      modal.classList.add('flex');
      document.body.classList.add('quick-view-open');
      createLucideIcons(modal);
    });

    modal.addEventListener('click', (event) => {
      if (event.target === modal || event.target.closest('[data-quick-view-close]')) {
        closeModal();
      }
    });

    document.addEventListener('keydown', (event) => {
      if (event.key === 'Escape') {
        closeModal();
      }
    });
  }

  function readCart() {
    try {
      const parsed = JSON.parse(window.localStorage.getItem(CART_STORAGE_KEY) || '[]');
      return Array.isArray(parsed) ? parsed : [];
    } catch {
      return [];
    }
  }

  function saveCart(cart) {
    window.localStorage.setItem(CART_STORAGE_KEY, JSON.stringify(cart));
  }

  function formatCurrency(value) {
    return `${Math.round(Number(value) || 0).toLocaleString('ar-IQ')} د.ع`;
  }

  function escapeHtml(value) {
    return String(value || '')
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#039;');
  }

  function getCartItemFromTrigger(trigger) {
    return {
      id: trigger.dataset.id,
      title: trigger.dataset.title || 'منتج',
      price: Number(trigger.dataset.price || 0),
      displayPrice: trigger.dataset.displayPrice || formatCurrency(trigger.dataset.price),
      image: trigger.dataset.image || '/images/product-placeholder.svg',
      store: trigger.dataset.store || 'SmartStore',
      url: trigger.dataset.url || '#',
      quantity: 1
    };
  }

  function renderCart() {
    const cart = readCart();
    const itemCount = cart.reduce((total, item) => total + Number(item.quantity || 0), 0);
    const totalAmount = cart.reduce((total, item) => total + (Number(item.price || 0) * Number(item.quantity || 0)), 0);

    document.querySelectorAll('[data-cart-count]').forEach((badge) => {
      badge.textContent = itemCount.toLocaleString('ar-IQ');
      badge.classList.toggle('hidden', itemCount === 0);
    });

    document.querySelectorAll('[data-cart-total]').forEach((element) => {
      element.textContent = formatCurrency(totalAmount);
    });

    document.querySelectorAll('[data-cart-empty]').forEach((element) => {
      element.classList.toggle('hidden', cart.length > 0);
    });

    document.querySelectorAll('[data-cart-items]').forEach((container) => {
      container.innerHTML = cart.map((item) => `
        <article class="rounded-3xl border border-slate-200 bg-white p-4 shadow-sm">
          <div class="flex gap-4">
            <a href="${escapeHtml(item.url)}" class="block h-20 w-20 shrink-0 overflow-hidden rounded-2xl bg-slate-100">
              <img src="${escapeHtml(item.image)}" alt="${escapeHtml(item.title)}" class="h-full w-full object-cover" loading="lazy" />
            </a>
            <div class="min-w-0 flex-1">
              <a href="${escapeHtml(item.url)}" class="line-clamp-2 text-sm font-black text-slate-950 hover:text-brand-700">${escapeHtml(item.title)}</a>
              <p class="mt-1 text-xs font-bold text-slate-500">${escapeHtml(item.store)}</p>
              <p class="mt-2 text-lg font-black text-slate-950">${formatCurrency(item.price)}</p>
            </div>
          </div>
          <div class="mt-4 flex items-center justify-between gap-3">
            <div class="inline-flex items-center rounded-2xl border border-slate-200 bg-slate-50">
              <button type="button" class="grid h-10 w-10 place-items-center text-slate-600 hover:text-brand-700" data-cart-increment="${escapeHtml(item.id)}" aria-label="زيادة الكمية">+</button>
              <span class="min-w-10 text-center text-sm font-black text-slate-950">${Number(item.quantity || 0).toLocaleString('ar-IQ')}</span>
              <button type="button" class="grid h-10 w-10 place-items-center text-slate-600 hover:text-brand-700" data-cart-decrement="${escapeHtml(item.id)}" aria-label="تقليل الكمية">-</button>
            </div>
            <button type="button" class="rounded-2xl border border-slate-200 px-4 py-2 text-xs font-black text-slate-500 hover:border-red-300 hover:text-red-700" data-cart-remove="${escapeHtml(item.id)}">حذف</button>
          </div>
        </article>
      `).join('');
    });

    createLucideIcons(document);
  }

  function openCart() {
    const drawer = document.querySelector('[data-cart-drawer]');
    if (!drawer) {
      return;
    }

    drawer.classList.remove('hidden');
    document.body.classList.add('quick-view-open');
  }

  function closeCart() {
    document.querySelector('[data-cart-drawer]')?.classList.add('hidden');
    document.body.classList.remove('quick-view-open');
  }

  function addItemToCart(item) {
    if (!item.id) {
      return;
    }

    const cart = readCart();
    const existing = cart.find((cartItem) => String(cartItem.id) === String(item.id));
    if (existing) {
      existing.quantity = Number(existing.quantity || 0) + 1;
    } else {
      cart.push(item);
    }

    saveCart(cart);
    renderCart();
    openCart();
  }

  function updateCartItem(id, change) {
    const cart = readCart();
    const item = cart.find((cartItem) => String(cartItem.id) === String(id));
    if (!item) {
      return;
    }

    item.quantity = Number(item.quantity || 0) + change;
    const nextCart = cart.filter((cartItem) => Number(cartItem.quantity || 0) > 0);
    saveCart(nextCart);
    renderCart();
  }

  function removeCartItem(id) {
    saveCart(readCart().filter((item) => String(item.id) !== String(id)));
    renderCart();
  }

  function initCart() {
    if (!initialized.cart) {
      initialized.cart = true;

      document.addEventListener('click', (event) => {
        const addTrigger = event.target.closest('[data-add-to-cart]');
        if (addTrigger) {
          event.preventDefault();
          addItemToCart(getCartItemFromTrigger(addTrigger));
          return;
        }

        if (event.target.closest('[data-cart-open]')) {
          event.preventDefault();
          openCart();
          return;
        }

        if (event.target.closest('[data-cart-close]')) {
          event.preventDefault();
          closeCart();
          return;
        }

        const increment = event.target.closest('[data-cart-increment]');
        if (increment) {
          updateCartItem(increment.dataset.cartIncrement, 1);
          return;
        }

        const decrement = event.target.closest('[data-cart-decrement]');
        if (decrement) {
          updateCartItem(decrement.dataset.cartDecrement, -1);
          return;
        }

        const remove = event.target.closest('[data-cart-remove]');
        if (remove) {
          removeCartItem(remove.dataset.cartRemove);
          return;
        }

        if (event.target.closest('[data-cart-clear]')) {
          saveCart([]);
          renderCart();
          return;
        }

        if (event.target.closest('[data-cart-checkout]')) {
          window.alert('تم حفظ المنتجات في السلة مؤقتاً. سيتم ربط متابعة الطلب بمرحلة الطلبات لاحقاً.');
        }
      });

      document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape') {
          closeCart();
        }
      });
    }

    renderCart();
  }

  function initCounters(root) {
    root.querySelectorAll('[data-counter]').forEach((element) => {
      if (element.dataset.counterReady) {
        return;
      }

      element.dataset.counterReady = 'true';
      const target = Number(element.dataset.counter || '0');
      if (!window.gsap) {
        element.textContent = target.toLocaleString('ar-IQ');
        return;
      }

      const state = { value: 0 };
      window.gsap.to(state, {
        value: target,
        duration: 1.1,
        ease: 'power3.out',
        onUpdate: () => {
          element.textContent = Math.round(state.value).toLocaleString('ar-IQ');
        }
      });
    });
  }

  function initCharts(root) {
    if (!window.Chart) {
      return;
    }

    const inventory = root.querySelector('#inventory-chart');
    if (inventory && !inventory.dataset.chartReady) {
      inventory.dataset.chartReady = 'true';
      new window.Chart(inventory, {
        type: 'doughnut',
        data: {
          labels: ['في المخزون', 'نفد المخزون', 'منخفض'],
          datasets: [{
            data: [
              Number(inventory.dataset.inStock || 0),
              Number(inventory.dataset.outStock || 0),
              Number(inventory.dataset.lowStock || 0)
            ],
            backgroundColor: ['#2563eb', '#ef4444', '#38bdf8'],
            borderWidth: 0
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          cutout: '68%',
          plugins: {
            legend: {
              position: 'bottom',
              labels: {
                usePointStyle: true,
                font: { family: 'Tahoma' }
              }
            }
          }
        }
      });
    }

    const activity = root.querySelector('#activity-chart');
    if (activity && !activity.dataset.chartReady) {
      activity.dataset.chartReady = 'true';
      new window.Chart(activity, {
        type: 'bar',
        data: {
          labels: ['مشاهدات', 'طلبات', 'منتجات'],
          datasets: [{
            label: 'النشاط',
            data: [
              Number(activity.dataset.views || 0),
              Number(activity.dataset.orders || 0),
              Number(activity.dataset.products || 0)
            ],
            backgroundColor: ['#1d4ed8', '#38bdf8', '#0f172a'],
            borderRadius: 14
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            y: {
              beginAtZero: true,
              grid: { color: '#e2e8f0' }
            },
            x: {
              grid: { display: false }
            }
          },
          plugins: {
            legend: { display: false }
          }
        }
      });
    }
  }

  function initTableFilters() {
    if (initialized.tableFilter) {
      return;
    }

    initialized.tableFilter = true;
    document.addEventListener('input', (event) => {
      const input = event.target.closest('[data-table-filter]');
      if (!input) {
        return;
      }

      const table = document.querySelector(input.dataset.tableFilter);
      const query = input.value.trim().toLowerCase();
      table?.querySelectorAll('tbody tr').forEach((row) => {
        row.hidden = query.length > 0 && !row.textContent.toLowerCase().includes(query);
      });
    });
  }

  function initGsapMotion(root) {
    if (!window.gsap || window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
      return;
    }

    root.querySelectorAll('.hero-copy, .hero-device').forEach((element) => {
      if (element.dataset.motionReady) {
        return;
      }

      element.dataset.motionReady = 'true';
      window.gsap.fromTo(element, { y: 24, opacity: 0 }, { y: 0, opacity: 1, duration: 0.9, ease: 'power3.out' });
    });
  }

  function boot(root) {
    const scope = root || document;
    createLucideIcons(scope);
    initAos();
    initHeroSwiper(scope);
    initProductSwipers(scope);
    initProductGallery(scope);
    initProductThumbs(scope);
    initTomSelect(scope);
    initQuickView();
    initCart();
    initCounters(scope);
    initCharts(scope);
    initTableFilters();
    initGsapMotion(scope);
  }

  document.addEventListener('DOMContentLoaded', () => boot(document));
  document.body?.addEventListener('htmx:afterSwap', (event) => boot(event.detail.target));
})();
