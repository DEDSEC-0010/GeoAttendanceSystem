function initGlobe() {
    // Scene setup
    const scene = new THREE.Scene();
    const camera = new THREE.PerspectiveCamera(75, 1, 0.1, 1000);
    const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });

    // Custom angled view
    camera.position.set(4, 3, 8);

    // Configure renderer
    const container = document.getElementById('globe-container');
    renderer.setSize(600, 600);
    renderer.setPixelRatio(window.devicePixelRatio);
    container.appendChild(renderer.domElement);

    // Add stars background
    const starsGeometry = new THREE.BufferGeometry();
    const starsMaterial = new THREE.PointsMaterial({
        color: 0xFFFFFF,
        size: 0.1
    });

    const starsVertices = [];
    for (let i = 0; i < 10000; i++) {
        starsVertices.push(
            THREE.MathUtils.randFloatSpread(2000),
            THREE.MathUtils.randFloatSpread(2000),
            THREE.MathUtils.randFloatSpread(2000)
        );
    }

    starsGeometry.setAttribute('position',
        new THREE.Float32BufferAttribute(starsVertices, 3));
    const starField = new THREE.Points(starsGeometry, starsMaterial);
    scene.add(starField);

    // Create Earth
    const globeGeometry = new THREE.SphereGeometry(3, 64, 64);
    const globeMaterial = new THREE.MeshPhongMaterial({
        map: new THREE.TextureLoader().load('https://raw.githubusercontent.com/mrdoob/three.js/master/examples/textures/planets/earth_atmos_2048.jpg'),
        normalMap: new THREE.TextureLoader().load('https://raw.githubusercontent.com/mrdoob/three.js/master/examples/textures/planets/earth_normal_2048.jpg'),
        specular: new THREE.Color('grey'),
        shininess: 5
    });

    const earth = new THREE.Mesh(globeGeometry, globeMaterial);
    scene.add(earth);

    // Add cloud layer
    const cloudGeometry = new THREE.SphereGeometry(3.02, 64, 64);
    const cloudMaterial = new THREE.MeshPhongMaterial({
        map: new THREE.TextureLoader().load('https://raw.githubusercontent.com/mrdoob/three.js/master/examples/textures/planets/earth_clouds_1024.png'),
        transparent: true,
        opacity: 0.8
    });

    const clouds = new THREE.Mesh(cloudGeometry, cloudMaterial);
    scene.add(clouds);

    // Lighting
    const ambientLight = new THREE.AmbientLight(0xffffff, 0.5);
    scene.add(ambientLight);

    const pointLight = new THREE.PointLight(0xffffff, 1);
    pointLight.position.set(10, 5, 10);
    scene.add(pointLight);

    // Add markers for office locations
    const markers = [
        { lat: 51.5074, lon: -0.1278, color: 0xff0000 }, // London
        { lat: 40.7128, lon: -74.0060, color: 0x00ff00 }, // New York
        { lat: 28.6139, lon: 77.2090, color: 0x0000ff }   // New Delhi
    ];

    markers.forEach(marker => {
        const phi = (90 - marker.lat) * (Math.PI / 180);
        const theta = (marker.lon + 180) * (Math.PI / 180);

        const markerGeometry = new THREE.SphereGeometry(0.05, 32, 32);
        const markerMaterial = new THREE.MeshBasicMaterial({ color: marker.color });
        const markerMesh = new THREE.Mesh(markerGeometry, markerMaterial);

        markerMesh.position.set(
            -Math.sin(phi) * Math.cos(theta) * 3.1,
            Math.cos(phi) * 3.1,
            Math.sin(phi) * Math.sin(theta) * 3.1
        );

        scene.add(markerMesh);
    });

    // Animation
    let animate = true;

    function animateGlobe() {
        requestAnimationFrame(animateGlobe);

        if (animate) {
            earth.rotation.y += 0.001;
            clouds.rotation.y += 0.0015;
        }

        renderer.render(scene, camera);
    }

    // Add controls for interactivity
    const controls = new THREE.OrbitControls(camera, renderer.domElement);
    controls.enableZoom = true;
    controls.enablePan = false;
    controls.enableDamping = true;
    controls.dampingFactor = 0.05;
    controls.rotateSpeed = 0.5;

    // Handle window resize
    window.addEventListener('resize', () => {
        camera.aspect = 1;
        camera.updateProjectionMatrix();
        renderer.setSize(600, 600);
    });

    // Start animation
    animateGlobe();

    // Toggle animation on interaction
    controls.addEventListener('start', () => animate = false);
    controls.addEventListener('end', () => animate = true);
}

// Initialize when document is loaded
document.addEventListener('DOMContentLoaded', initGlobe);